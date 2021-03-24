using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bamboozed.Application.Context.Interfaces
{
    public interface IConversationReferenceContext
    {
        ConversationReference Context { get; set; }
    }
}
