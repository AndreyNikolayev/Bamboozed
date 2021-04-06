using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bamboozed.Application.Commands.Exceptions;
using Bamboozed.Application.Commands.Interfaces;
using CommandLine;

namespace Bamboozed.Application.Commands.Services
{
    public class CommandParser
    {
        private static readonly Type[] CommandTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(ICommand).IsAssignableFrom(p) && !p.IsAbstract)
            .ToArray();

        private static readonly Parser Parser = new Parser(settings =>
        {
            settings.AutoHelp = false;
            settings.CaseInsensitiveEnumValues = true;
        });

        public ICommand GetCommand(string commandText)
        {
            ICommand result = null;

            Parser
                .ParseArguments(ParseArguments(commandText), CommandTypes)
                .WithNotParsed(errs => throw new CommandNotParsedException("Command not parsed"))
                .WithParsed(parsed => result = (ICommand)parsed);

            return result;
        }

        private static IEnumerable<string> ParseArguments(string commandLine)
        {
            if (string.IsNullOrWhiteSpace(commandLine))
                yield break;

            var sb = new StringBuilder();
            var inQuote = false;
            foreach (var c in commandLine)
            {
                if (c == '\'' && !inQuote)
                {
                    inQuote = true;
                    continue;
                }

                if (c != '\'' && !(char.IsWhiteSpace(c) && !inQuote))
                {
                    sb.Append(c);
                    continue;
                }

                if (sb.Length <= 0) continue;
                var result = sb.ToString();
                sb.Clear();
                inQuote = false;
                yield return result;
            }

            if (sb.Length > 0)
                yield return sb.ToString();
        }
    }
}
