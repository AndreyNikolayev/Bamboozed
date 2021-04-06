using System;
using System.Threading.Tasks;
using Bamboozed.Application.Events.Interfaces;
using Bamboozed.Application.Services;
using Bamboozed.DAL.Repository;
using Bamboozed.Domain.Extensions;
using Bamboozed.Domain.NotificationRequest;
using Bamboozed.Domain.TimeOffPolicy;
using Bamboozed.Domain.User;
using CSharpFunctionalExtensions;

namespace Bamboozed.Application.Events.Events.Policy
{
    public sealed class PolicyDeletedEvent: IDomainEvent
    {
        public UserTimeOffPolicy UserTimeOffPolicy { get; }

        public PolicyDeletedEvent(UserTimeOffPolicy userTimeOffPolicy)
        {
            UserTimeOffPolicy = userTimeOffPolicy ?? throw new ArgumentNullException($"{nameof(userTimeOffPolicy)} cannot be null");
        }
    }

    public sealed class PolicyDeletedEventHandler : IEventHandler<PolicyDeletedEvent>
    {
        private readonly NotificationService _notificationService;
        private readonly IRepository<User> _userRepository;

        public PolicyDeletedEventHandler(NotificationService notificationService,
            IRepository<User> userRepository)
        {
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public Task Handle(PolicyDeletedEvent domainEvent)
        {
            return _userRepository.GetById(domainEvent.UserTimeOffPolicy.UserEmail)
                .ToResult($"User with email {domainEvent.UserTimeOffPolicy.UserEmail} is not found.")
                .Tap(user => _notificationService.Notify(new NotificationRequest(
                        user.ConversationId,
                        $"Policy for {domainEvent.UserTimeOffPolicy.TimeOffType.GetDescription()} is removed."
                    )
                ));
        }
    }
}
