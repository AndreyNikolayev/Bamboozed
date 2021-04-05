using System.Threading.Tasks;
using Bamboozed.Domain;
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

        public NotifyController(IBotFrameworkHttpAdapter adapter, IConfiguration configuration)
        {
            _adapter = adapter;
            _appId = configuration["MicrosoftAppId"] ?? string.Empty;
        }

        public async Task<IActionResult> Get([FromBody] NotificationRequest request)
        {

            AppCredentials.TrustServiceUrl(request.ConversationReference.ServiceUrl);

            await ((BotAdapter)_adapter).ContinueConversationAsync(_appId,
                request.ConversationReference,
                async (turnContext, cancellationToken) => await turnContext.SendActivityAsync(request.Message, cancellationToken: cancellationToken),
            default);

            return new OkResult();
        }
    }
}
