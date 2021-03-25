using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Bamboozed.Application.Commands.Exceptions;
using Bamboozed.Application.Commands.Interfaces;
using Bamboozed.Application.Context.Interfaces;
using Bamboozed.Application.Interfaces;
using Bamboozed.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bamboozed.AzureFunctions
{
    public class Facade
    {
        private readonly ICommandParser _commandParser;
        private readonly ICommandBus _commandBus;
        private readonly IConversationReferenceContext _conversationReferenceContext;

        public Facade(ICommandParser commandParser,
            ICommandBus commandBus,
            IConversationReferenceContext conversationReferenceContext)
        {
            _commandParser = commandParser;
            _commandBus = commandBus;
            _conversationReferenceContext = conversationReferenceContext;
        }

        [FunctionName("Facade")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                using var reader = new StreamReader(req.Body);
                var bodyJson = await reader.ReadToEndAsync();

                var notificationRequest = JsonConvert.DeserializeObject<NotificationRequest>(bodyJson);
                _conversationReferenceContext.Context = notificationRequest.ConversationReference;

                var command = _commandParser.GetCommand(notificationRequest.Message);

                var commandResult = await _commandBus.Handle(command);

                return new OkObjectResult(commandResult.Message);
            }
            catch (CommandNotParsedException)
            {
                return new OkObjectResult("Invalid command");
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);

                return new OkObjectResult("Something went wrong");
            }
        }
    }

}
