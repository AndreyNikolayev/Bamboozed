using System;
using System.Threading.Tasks;
using Bamboozed.Application.Events.Interfaces;
using Bamboozed.Application.Services;
using Bamboozed.Domain.NotificationRequest;

namespace Bamboozed.Application.Events.Events
{
    public sealed class RegistrationStepFailedEvent: IDomainEvent
    {
        public string ConversationId { get; }
        public string Message { get; }

        public RegistrationStepFailedEvent(string conversationId, string message)
        {
            ConversationId = conversationId ?? throw new ArgumentNullException($"{nameof(conversationId)} cannot be null");
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
            return _notificationService.Notify(new NotificationRequest(domainEvent.ConversationId,
                domainEvent.Message));
        }
    }
}
