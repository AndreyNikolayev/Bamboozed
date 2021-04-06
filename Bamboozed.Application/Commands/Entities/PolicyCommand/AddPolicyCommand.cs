using System.Linq;
using System.Threading.Tasks;
using Bamboozed.Application.Commands.Interfaces;
using Bamboozed.Application.Events;
using Bamboozed.Application.Events.Events.Policy;
using Bamboozed.DAL.Repository;
using Bamboozed.Domain.Extensions;
using Bamboozed.Domain.TimeOffPolicy;
using Bamboozed.Domain.TimeOffRequest;
using Bamboozed.Domain.User;
using CommandLine;
using CSharpFunctionalExtensions;

namespace Bamboozed.Application.Commands.Entities.PolicyCommand
{
    [Verb("policy", HelpText = "Add/Edit Policy for auto-handling of time-off approval/denial")]
    public class AddPolicyCommand : ICommand
    {
        [Option('a', "action", Required = true, HelpText = "approve or deny")]
        public TimeOffAction TimeOffAction { get; set; }
        [Option('t', "type", Required = true, HelpText = "sickLeave or dayOff or vacation")]
        public TimeOffType TimeOffType { get; set; }
        [Option('d', "max-days", Required = true, HelpText = "max amount of consecutive days for the policy action")]
        public int MaxDays { get; set; }
        public string ConversationId { get; set; }
    }

    public class AddPolicyCommandHandler : ICommandHandler<AddPolicyCommand>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<MaxDaysOffPolicy> _maxDaysOffPolicyRepository;
        private readonly DomainEventBus _domainEventBus;

        public AddPolicyCommandHandler(IRepository<User> userRepository,
            IRepository<MaxDaysOffPolicy> maxDaysOffPolicyRepository,
            DomainEventBus domainEventBus)
        {
            _userRepository = userRepository;
            _maxDaysOffPolicyRepository = maxDaysOffPolicyRepository;
            _domainEventBus = domainEventBus;
        }

        public async Task<Result> Handle(AddPolicyCommand command)
        {
            return await _userRepository.Get()
                .ToResultTask()
                .Map(users => users.FirstOrDefault(p => p.ConversationId == command.ConversationId))

                .Ensure(user => user != null && user.UserStatus == UserStatus.Active,
                    "Complete registration before working with policies"
                )
                .Map(async user =>
                {
                    var policy = (await _maxDaysOffPolicyRepository.Get())
                        .FirstOrDefault(p => p.UserEmail == user.Email 
                                             && p.Action == command.TimeOffAction
                                             && p.TimeOffType == command.TimeOffType);

                    if (policy != null)
                    {
                        policy.ChangeMaxDays(command.MaxDays);
                        await _maxDaysOffPolicyRepository.Edit(policy);
                        return policy;
                    }
                    policy = new MaxDaysOffPolicy(user.Email, command.TimeOffAction, command.TimeOffType, command.MaxDays);
                    await _maxDaysOffPolicyRepository.Add(policy);
                    return policy;
                })
                .Tap(policy => _domainEventBus.Dispatch(new PolicyUpsertedEvent(policy)))
                .OnFailure(error => _domainEventBus.Dispatch(new PolicyChangeFailedEvent(command.ConversationId, error)))
                .Bind(_ => Result.Success());
        }
    }
}
