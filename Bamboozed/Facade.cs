using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Bamboozed.Application.Commands.Exceptions;
using Bamboozed.Application.Commands.Services;
using Bamboozed.Domain.NotificationRequest;
using CSharpFunctionalExtensions;
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
        private readonly CommandParser _commandParser;
        private readonly CommandBus _commandBus;

        public Facade(CommandParser commandParser,
            CommandBus commandBus)
        {
            _commandParser = commandParser;
            _commandBus = commandBus;
        }

        [FunctionName(nameof(Facade))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                using var reader = new StreamReader(req.Body);
                var bodyJson = await reader.ReadToEndAsync();

                var notificationRequest = JsonConvert.DeserializeObject<NotificationRequest>(bodyJson);

                var command = _commandParser.GetCommand(notificationRequest.Message);

                Maybe<PropertyInfo> conversationField = command.GetType().GetProperty("ConversationId");
                if (conversationField.HasValue)
                {
                    conversationField.Value.SetValue(command, notificationRequest.ConversationId, null);
                }

                await _commandBus.Handle(command);

                return new OkResult();
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
