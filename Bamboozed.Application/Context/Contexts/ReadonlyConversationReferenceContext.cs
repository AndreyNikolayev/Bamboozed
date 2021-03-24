using System;
using Bamboozed.Application.Context.Interfaces;
using Microsoft.Bot.Schema;

namespace Bamboozed.Application.Context.Contexts
{
    public class ReadonlyConversationReferenceContext: IReadonlyConversationReferenceContext
    {
        private readonly IConversationReferenceContext _conversationReferenceContext;

        public ReadonlyConversationReferenceContext(IConversationReferenceContext conversationReferenceContext)
        {
            _conversationReferenceContext = conversationReferenceContext;
        }

        public ConversationReference Context
        {
            get
            {
                if (_conversationReferenceContext.Context == null)
                {
                    throw new Exception("Request Context is not set");
                }

                return _conversationReferenceContext.Context;
            }
        }
    }
}
