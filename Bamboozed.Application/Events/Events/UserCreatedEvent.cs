using System;
using System.Threading.Tasks;
using Bamboozed.Application.Events.Interfaces;
using Bamboozed.Application.Services;
using Bamboozed.DAL.Repository;
using Bamboozed.Domain;
using Bamboozed.Domain.User;
using CSharpFunctionalExtensions;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Bamboozed.Application.Events.Events
{
    public sealed class UserCreatedEvent: IDomainEvent
    {
        public string Email { get; }

        public UserCreatedEvent(string email)
        {
            Email = email ?? throw new ArgumentNullException($"{nameof(email)} cannot be null");
        }
    }

    public sealed class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
    {
        private readonly NotificationService _notificationService;
        private readonly IRepository<User> _userRepository;

        public UserCreatedEventHandler(NotificationService notificationService,
            IRepository<User> userRepository)
        {
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public Task HandleAsync(UserCreatedEvent domainEvent)
        {
            return _userRepository.GetById(domainEvent.Email)
                .ToResult($"User with email {domainEvent.Email} is not found.")
                .Tap(user => _notificationService.Notify(new NotificationRequest(
                        JsonConvert.DeserializeObject<ConversationReference>(user.ConversationReferenceJson),
                        "Registration code is sent to your mailbox. Please submit it with 'code' command."
                    )
                ));
        }
    }
}
