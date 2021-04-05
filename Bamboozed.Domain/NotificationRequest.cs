using System;

namespace Bamboozed.Domain
{
    public class NotificationRequest
    {
        public string ConversationId { get; }
        public string Message { get; }

        public NotificationRequest(string conversationId, string message)
        {
            ConversationId = conversationId ?? throw new ArgumentNullException($"{nameof(conversationId)} cannot be null");
            Message = message ?? throw new ArgumentNullException($"{nameof(message)} cannot be null");
        }
    }
}
