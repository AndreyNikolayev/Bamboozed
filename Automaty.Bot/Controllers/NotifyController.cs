using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Bamboozed.Bot.Controllers
{
    [Route("api/notify")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly string _appId;
        private readonly ConversationReference _conversationReference;

        public NotifyController(IBotFrameworkHttpAdapter adapter, IConfiguration configuration)
        {
            _adapter = adapter;
            _appId = configuration["MicrosoftAppId"] ?? string.Empty;

            _conversationReference = configuration["ConversationReference"] == null ? null :
                JsonConvert.DeserializeObject<ConversationReference>(configuration["ConversationReference"]);
        }

        public async Task<IActionResult> Get([FromBody] string message)
        {
            AppCredentials.TrustServiceUrl(_conversationReference.ServiceUrl);

            await ((BotAdapter)_adapter).ContinueConversationAsync(_appId,
                _conversationReference,
                async (turnContext, cancellationToken) => await turnContext.SendActivityAsync(message, cancellationToken: cancellationToken),
            default);

            return new OkResult();
        }
    }
}
