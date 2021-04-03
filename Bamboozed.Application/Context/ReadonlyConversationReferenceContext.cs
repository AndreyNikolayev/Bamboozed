using System;
using Microsoft.Bot.Schema;

namespace Bamboozed.Application.Context
{
    public class ReadonlyConversationReferenceContext
    {
        private readonly ConversationReferenceContext _conversationReferenceContext;

        public ReadonlyConversationReferenceContext(ConversationReferenceContext conversationReferenceContext)
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
