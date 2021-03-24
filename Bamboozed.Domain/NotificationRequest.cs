using Microsoft.Bot.Schema;
using System;

namespace Bamboozed.Domain
{
    public class NotificationRequest
    {
        public ConversationReference ConversationReference { get; set; }
        public string Message { get; set; }
    }
}
