using System;
using System.Threading.Tasks;
using Bamboozed.Application.Events.Interfaces;
using Bamboozed.Application.Services;
using Bamboozed.DAL.Repository;
using Bamboozed.Domain;
using Bamboozed.Domain.User;
using CSharpFunctionalExtensions;

namespace Bamboozed.Application.Events.Events
{
    public sealed class PasswordSubmittedEvent: IDomainEvent
    {
        public string Email { get; }

        public PasswordSubmittedEvent(string email)
        {
            Email = email ?? throw new ArgumentNullException($"{nameof(email)} cannot be null");
        }
    }

    public sealed class PasswordSubmittedEventHandler : IEventHandler<PasswordSubmittedEvent>
    {
        private readonly NotificationService _notificationService;
        private readonly IRepository<User> _userRepository;

        public PasswordSubmittedEventHandler(NotificationService notificationService,
            IRepository<User> userRepository)
        {
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public Task Handle(PasswordSubmittedEvent domainEvent)
        {
            return _userRepository.GetById(domainEvent.Email)
                .ToResult($"User with email {domainEvent.Email} is not found.")
                .Tap(user => _notificationService.Notify(new NotificationRequest(
                        user.ConversationId,
                        "Your password is submitted successfully."
                    )
                ));
        }
    }
}
