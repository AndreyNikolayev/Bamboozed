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
    [Verb("policy", HelpText = "Add Policy for automating handling of approval/denial")]
    public class AddPolicyCommand : ICommand
    {
        public TimeOffAction TimeOffAction { get; set; }
        public TimeOffType TimeOffType { get; set; }
        public int MaxDays { get; set; }
        public string ConversationId { get; set; }
    }

    public class AddPolicyCommandHandler : ICommandHandler<AddPolicyCommand>
    {
        public Task<Result> Handle(AddPolicyCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
