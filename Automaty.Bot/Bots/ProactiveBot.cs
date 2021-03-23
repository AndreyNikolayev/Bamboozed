using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Activity = Microsoft.Bot.Schema.Activity;

namespace Bamboozed.Bot.Bots
{
    public class ProactiveBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var activity = turnContext.Activity as Activity;

            var conversationReference = activity.GetConversationReference();

            var sendMessage = string.Join(" ",activity.Text
                .Split(" ")
                .Where(p => !string.IsNullOrEmpty(p) &&
                            !p.Equals("AutomatifyBot", StringComparison.InvariantCultureIgnoreCase))
                .Select(p => p.Trim())
            );

            var replyActivity = activity.CreateReply(JsonConvert.SerializeObject(conversationReference));

            await turnContext.SendActivityAsync(replyActivity, cancellationToken);
        }
    }
}
