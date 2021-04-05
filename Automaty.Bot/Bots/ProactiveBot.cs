using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Bamboozed.Domain;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Activity = Microsoft.Bot.Schema.Activity;

namespace Bamboozed.Bot.Bots
{
    public class ProactiveBot : ActivityHandler
    {
        private readonly string _requestEndpoint;
        private readonly HttpClient _httpClient;

        public ProactiveBot(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _requestEndpoint = configuration["RequestEndpoint"];
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var activity = turnContext.Activity as Activity;

            if (string.IsNullOrWhiteSpace(activity.Text))
            {
                return;
            }

            var conversationReference = activity.GetConversationReference();

            var sendMessage = string.Join(" ",activity.Text
                .Split(" ")
                .Where(p => !string.IsNullOrEmpty(p) &&
                            !p.Equals("Bamboozed", StringComparison.InvariantCultureIgnoreCase))
                .Select(p => p.Trim())
            );

            var withoutHtmlMessage = Regex.Replace(sendMessage, "<.*?>", String.Empty);

            var requestBody = JsonConvert.SerializeObject(new NotificationRequest(
                conversationReference.Conversation.Id,
                withoutHtmlMessage
            ), new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            try
            {
                var result = await _httpClient.PostAsync(_requestEndpoint,
                    new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellationToken);

                var replyMessage = await result.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(replyMessage))
                {
                    return;
                }

                var replyActivity = activity.CreateReply(replyMessage);
                await turnContext.SendActivityAsync(replyActivity, cancellationToken);
            }
            catch (Exception e)
            {
                var replyActivity = activity.CreateReply(e.Message);
                await turnContext.SendActivityAsync(replyActivity, cancellationToken);
            }
        }
    }
}
