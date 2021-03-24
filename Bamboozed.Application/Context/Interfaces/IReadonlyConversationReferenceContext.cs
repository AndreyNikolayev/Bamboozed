using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bamboozed.Application.Context.Interfaces
{
    public interface IReadonlyConversationReferenceContext
    {
        ConversationReference Context { get; }
    }
}
