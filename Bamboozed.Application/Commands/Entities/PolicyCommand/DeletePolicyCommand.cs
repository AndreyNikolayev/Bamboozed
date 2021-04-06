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
    [Verb("remove-policy", HelpText = "Remove Policy for auto-handling of time-off approval/denial")]
    public class RemovePolicyCommand : ICommand
    {
        [Option('a', "action", Required = true, HelpText = "approve or deny")]
        public TimeOffAction TimeOffAction { get; set; }
        [Option('t', "type", Required = true, HelpText = "sick-leave or day-off or vacation")]
        public TimeOffType TimeOffType { get; set; }
        public string ConversationId { get; set; }
    }

    public class RemovePolicyCommandHandler : ICommandHandler<RemovePolicyCommand>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<MaxDaysOffPolicy> _maxDaysOffPolicyRepository;
        private readonly DomainEventBus _domainEventBus;

        public RemovePolicyCommandHandler(IRepository<User> userRepository,
            IRepository<MaxDaysOffPolicy> maxDaysOffPolicyRepository,
            DomainEventBus domainEventBus)
        {
            _userRepository = userRepository;
            _maxDaysOffPolicyRepository = maxDaysOffPolicyRepository;
            _domainEventBus = domainEventBus;
        }


        public async Task<Result> Handle(RemovePolicyCommand command)
        {
            return await _userRepository.Get()
                .ToResultTask()
                .Map(users => users.FirstOrDefault(p => p.ConversationId == command.ConversationId))

                .Ensure(user => user != null && user.UserStatus == UserStatus.Active,
                    "Complete registration before working with policies"
                )
                .Map(async user => (await _maxDaysOffPolicyRepository.Get())
                                        .FirstOrDefault(p => 
                                            p.UserEmail == user.Email
                                         && p.Action == command.TimeOffAction
                                         && p.TimeOffType == command.TimeOffType)
                 )
                .Ensure(policy => policy != null, "Policy is not found.")
                .Tap(policy => _maxDaysOffPolicyRepository.Delete(policy))
                .Tap(policy => _domainEventBus.Dispatch(new PolicyDeletedEvent(policy)))
                .OnFailure(error => _domainEventBus.Dispatch(new PolicyChangeFailedEvent(command.ConversationId, error)))
                .Bind(_ => Result.Success());
        }
    }
}
