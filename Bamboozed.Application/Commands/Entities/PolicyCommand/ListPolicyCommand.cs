using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bamboozed.Application.Commands.Interfaces;
using Bamboozed.Application.Services;
using Bamboozed.DAL.Repository;
using Bamboozed.Domain.Extensions;
using Bamboozed.Domain.NotificationRequest;
using Bamboozed.Domain.TimeOffPolicy;
using Bamboozed.Domain.User;
using CommandLine;
using CSharpFunctionalExtensions;

namespace Bamboozed.Application.Commands.Entities.PolicyCommand
{
    [Verb("ls-policy", HelpText = "Get list of your time-off approval policies")]
    public class ListPolicyCommand : ICommand
    {
        public string ConversationId { get; set; }
    }

    public class ListPolicyCommandHandler : ICommandHandler<ListPolicyCommand>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<MaxDaysOffPolicy> _maxDaysOffPolicyRepository;
        private readonly NotificationService _notificationService;

        public ListPolicyCommandHandler(
            IRepository<MaxDaysOffPolicy> maxDaysOffPolicyRepository,
            NotificationService notificationService,
            IRepository<User> userRepository)
        {
            _maxDaysOffPolicyRepository = maxDaysOffPolicyRepository;
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public async Task<Result> Handle(ListPolicyCommand command)
        {
            return await _userRepository.Get()
                .ToResultTask()
                .Map(users => users.FirstOrDefault(p => p.ConversationId == command.ConversationId))

                .Ensure(user => user != null && user.UserStatus == UserStatus.Active,
                    "Complete registration before working with policies"
                )
                .Map(async user =>
                {
                    return (await _maxDaysOffPolicyRepository.Get())
                        .Where(p => p.UserEmail == user.Email)
                        .ToList();
                })
                .Tap(policies => _notificationService.Notify(new NotificationRequest(command.ConversationId, GetPoliciesDescription(policies))))
                .OnFailure(error => _notificationService.Notify(new NotificationRequest(command.ConversationId, error)))
                .Bind(_ => Result.Success());
        }

        private string GetPoliciesDescription(IReadOnlyCollection<MaxDaysOffPolicy> daysOffPolicies)
        {
            if (!daysOffPolicies.Any())
            {
                return "You don't have any policies yet";
            }

            var result = new StringBuilder();

            result.AppendLine("Your Policies:");
            result.AppendLine();

            foreach (var daysOffPolicy in daysOffPolicies)
            {
                result.AppendLine(
                    $"{daysOffPolicy.Action.GetDescription()} {daysOffPolicy.TimeOffType}s if no more than {daysOffPolicy.MaxDays} consecutive days");
            }

            return result.ToString();
        }
    }
}
