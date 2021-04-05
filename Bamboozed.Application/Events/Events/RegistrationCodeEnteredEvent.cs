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
    public sealed class RegistrationCodeEnteredEvent: IDomainEvent
    {
        public string Email { get; }

        public RegistrationCodeEnteredEvent(string email)
        {
            Email = email ?? throw new ArgumentNullException($"{nameof(email)} cannot be null");
        }
    }

    public sealed class RegistrationCodeEnteredEventHandler : IEventHandler<RegistrationCodeEnteredEvent>
    {
        private readonly NotificationService _notificationService;
        private readonly IRepository<User> _userRepository;

        public RegistrationCodeEnteredEventHandler(NotificationService notificationService,
            IRepository<User> userRepository)
        {
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public Task Handle(RegistrationCodeEnteredEvent domainEvent)
        {
            return _userRepository.GetById(domainEvent.Email)
                .ToResult($"User with email {domainEvent.Email} is not found.")
                .Tap(user => _notificationService.Notify(new NotificationRequest(
                        user.ConversationId,
                        "Code is verified. Please now submit your Bamboo password with 'password' command."
                    )
                ));
        }
    }
}
