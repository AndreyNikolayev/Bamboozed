using Bamboozed.Application.Context.Interfaces;
using Microsoft.Bot.Schema;

namespace Bamboozed.Application.Context.Contexts
{
    public class ConversationReferenceContext: IConversationReferenceContext
    {
       public ConversationReference Context { get; set; }
    }
}
