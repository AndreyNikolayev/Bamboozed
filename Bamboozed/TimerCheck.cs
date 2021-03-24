using System.Threading.Tasks;
using Bamboozed.Application.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Bamboozed.AzureFunctions
{
    public class TimerCheck
    {
        private readonly ITimeOffService _timeOffService;

        public TimerCheck(ITimeOffService timeOffService)
        {
            _timeOffService = timeOffService;
        }

        [FunctionName(nameof(TimerCheck))]
        public Task Run([TimerTrigger("0 0 * * * *")]TimerInfo myTimer, ILogger log)
        {
            return _timeOffService.Handle();
        }
    }
}
