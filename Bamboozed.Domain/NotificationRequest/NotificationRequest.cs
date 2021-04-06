using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace Bamboozed.Domain.NotificationRequest
{
    public class NotificationRequest: ValueObject
    {
        public string ConversationId { get; }
        public string Message { get; }

        public NotificationRequest(string conversationId, string message)
        {
            ConversationId = conversationId ?? throw new ArgumentNullException($"{nameof(conversationId)} cannot be null");
            Message = message ?? throw new ArgumentNullException($"{nameof(message)} cannot be null");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ConversationId;
            yield return Message;
        }
    }
}
