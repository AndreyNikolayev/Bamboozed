using System.Threading.Tasks;
using Bamboozed.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace Bamboozed.Bot.Controllers
{
    [Route("api/notify")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly string _appId;
        private readonly string _serviceUrl;

        public NotifyController(IBotFrameworkHttpAdapter adapter, IConfiguration configuration)
        {
            _adapter = adapter;
            _appId = configuration["MicrosoftAppId"] ?? string.Empty;
            _appId = configuration["ServiceUrl"] ?? string.Empty;
        }

        public async Task<IActionResult> Get([FromBody] NotificationRequest request)
        {

            AppCredentials.TrustServiceUrl(_serviceUrl);

            await ((BotAdapter)_adapter).ContinueConversationAsync(_appId,
                new ConversationReference(
                    conversation: new ConversationAccount(id: request.ConversationId),
                    serviceUrl: _serviceUrl
                ),
                async (turnContext, cancellationToken) => await turnContext.SendActivityAsync(request.Message, cancellationToken: cancellationToken),
            default);

            return new OkResult();
        }
    }
}
