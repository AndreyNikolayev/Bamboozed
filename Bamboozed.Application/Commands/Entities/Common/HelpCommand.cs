using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bamboozed.Application.Commands.Interfaces;
using Bamboozed.Application.Services;
using Bamboozed.Domain.NotificationRequest;
using CommandLine;
using CSharpFunctionalExtensions;

namespace Bamboozed.Application.Commands.Entities.Common
{
    [Verb("help")]
    public class HelpCommand: ICommand
    {
        [Value(0, HelpText = "Command Name")]
        public string CommandName { get; set; }
        public string ConversationId { get; set; }
    }

    public class HelpCommandHandler : ICommandHandler<HelpCommand>
    {
        private readonly NotificationService _notificationService;

        public HelpCommandHandler(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public Task<Result> Handle(HelpCommand command)
        {
            return Result.Success(AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => typeof(ICommand).IsAssignableFrom(p)
                                && p != typeof(HelpCommand)
                                && !p.IsAbstract
                    )
                    .OrderBy(p => p.GetCustomAttribute<VerbAttribute>()?.Name)
                    .ToArray())
                .Ensure(
                    types => command.CommandName == null || types.Any(p =>
                        p.GetCustomAttribute<VerbAttribute>()?.Name == command.CommandName),
                    $"Command '{command.CommandName}' not found. Type 'help' to see list of available commands")
                .Map(types => command.CommandName == null ?
                    GetAllCommandsHelpText(types) : GetCommandHelpText(types, command.CommandName))
                .Finally(async result =>
                {
                    await _notificationService.Notify(new NotificationRequest(command.ConversationId,
                        result.IsSuccess ? result.Value : result.Error));

                    return (Result)result;
                });
        }

        private static string GetAllCommandsHelpText(IEnumerable<Type> types)
        {
            var result = new StringBuilder();
            result.AppendLine("Available commands:");
            result.AppendLine();

            foreach (var type in types)
            {
                var verbAttribute = type.GetCustomAttribute<VerbAttribute>();

                result.AppendLine($"{verbAttribute.Name?.PadRight(22)} {verbAttribute.HelpText}");
            }

            return result.ToString();
        }

        private static string GetCommandHelpText(IEnumerable<Type> types, string commandName)
        {
            var type = types.First(p => p.GetCustomAttribute<VerbAttribute>()?.Name == commandName);
            var result = new StringBuilder();

            result.AppendLine(type.GetCustomAttribute<VerbAttribute>().HelpText);

            var valueLines = type.GetProperties()
                .Select(p => p.GetCustomAttribute<ValueAttribute>())
                .Where(p => p != null)
                .Select(p => $"${p.Index} \t{p.HelpText}")
                .ToList();

            var optionSelector = new Func<OptionAttribute, string>(p =>
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.Append(!string.IsNullOrEmpty(p.ShortName) ? $"-{p.ShortName}" : $"--{p.LongName}");

                stringBuilder.Append($"\t{p.HelpText}");

                return stringBuilder.ToString();
            });

            var optionLines = type.GetProperties()
                .Select(p => p.GetCustomAttribute<OptionAttribute>())
                .Where(p => p != null)
                .Select(optionSelector)
                .ToList();


            foreach (var optionLine in valueLines.Union(optionLines))
            {
                result.AppendLine(optionLine);
            }

            return result.ToString();
        }
    }
}
