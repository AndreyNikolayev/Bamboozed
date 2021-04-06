using System;
using System.Threading.Tasks;
using Bamboozed.Application.Commands.Interfaces;
using Bamboozed.Application.Services;
using Bamboozed.Domain.NotificationRequest;
using Bamboozed.Domain.TimeOffRequest;
using CommandLine;
using CSharpFunctionalExtensions;

namespace Bamboozed.Application.Commands.Entities
{
    [Verb("remove-policy", HelpText = "Remove Policy for automating handling of approval/denial")]
    public class RemovePolicyCommand : ICommand
    {
        public TimeOffAction TimeOffAction { get; set; }
        public TimeOffType TimeOffType { get; set; }
        public string ConversationId { get; set; }
    }

    public class RemovePolicyCommandHandler : ICommandHandler<RemovePolicyCommand>
    {
        public Task<Result> Handle(RemovePolicyCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
