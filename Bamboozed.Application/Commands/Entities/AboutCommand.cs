using System.Threading.Tasks;
using Bamboozed.Application.Commands.Interfaces;
using Bamboozed.Application.Services;
using Bamboozed.Domain;
using CommandLine;
using CSharpFunctionalExtensions;

namespace Bamboozed.Application.Commands.Entities
{
    [Verb("about", HelpText = "Get information about bot functionality")]
    public class AboutCommand : ICommand
    {
        public string ConversationId { get; set; }
    }

    public class AboutCommandHandler : ICommandHandler<AboutCommand>
    {
        private readonly NotificationService _notificationService;

        public AboutCommandHandler(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<Result> Handle(AboutCommand command)
        {
            await _notificationService.Notify(new NotificationRequest(command.ConversationId, AboutMessage));

            return Result.Success();
        }

        private const string AboutMessage = @"Bot for automating Bamboo day-off approvals.";
    }
}
