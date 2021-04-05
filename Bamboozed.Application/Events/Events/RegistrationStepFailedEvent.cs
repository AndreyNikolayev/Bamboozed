using System;
using System.Threading.Tasks;
using Bamboozed.Application.Events.Interfaces;
using Bamboozed.Application.Services;
using Bamboozed.Domain;
using Microsoft.Bot.Schema;

namespace Bamboozed.Application.Events.Events
{
    public sealed class RegistrationStepFailedEvent: IDomainEvent
    {
        public ConversationReference ConversationReference { get; }
        public string Message { get; }

        public RegistrationStepFailedEvent(ConversationReference conversationReference, string message)
        {
            ConversationReference = conversationReference ?? throw new ArgumentNullException($"{nameof(conversationReference)} cannot be null");
            Message = message ?? throw new ArgumentNullException($"{nameof(message)} cannot be null");
        }
    }

    public sealed class RegistrationStepFailedEventHandler : IEventHandler<RegistrationStepFailedEvent>
    {
        private readonly NotificationService _notificationService;

        public RegistrationStepFailedEventHandler(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public Task Handle(RegistrationStepFailedEvent domainEvent)
        {
            return _notificationService.Notify(new NotificationRequest(domainEvent.ConversationReference,
                domainEvent.Message));
        }
    }
}
