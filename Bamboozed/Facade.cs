using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
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
        [FunctionName("Facade")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                using var reader = new StreamReader(req.Body);
                var bodyJson = await reader.ReadToEndAsync();

                var notificationRequest = JsonConvert.DeserializeObject<NotificationRequest>(bodyJson);

                var decodedCommandText = HttpUtility.HtmlDecode(notificationRequest.Message);

                //var command = _commandParser.GetCommand(commandText);

                //var commandResult = await _commandBus.Handle(request.Command);

                return new OkObjectResult(commandResult.Message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);

                return new OkObjectResult("Something went wrong");
            }
        }
    }

}
