using System;
using Microsoft.Bot.Schema;

namespace Bamboozed.Domain
{
    public class NotificationRequest
    {
        public ConversationReference ConversationReference { get; }
        public string Message { get; }

        public NotificationRequest(ConversationReference conversationReference, string message)
        {
            ConversationReference = conversationReference ?? throw new ArgumentNullException($"{nameof(conversationReference)} cannot be null");
            Message = message ?? throw new ArgumentNullException($"{nameof(message)} cannot be null");
        }
    }
}
