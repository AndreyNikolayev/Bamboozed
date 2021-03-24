using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Bamboozed.Application;
using Bamboozed.Application.Interfaces;

namespace Bamboozed.Playground
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var collection = new ServiceCollection();
            ApplicationConfiguration.Setup(collection);
            var serviceProvider = collection.BuildServiceProvider();

            var timeOffService = serviceProvider.GetRequiredService<IMailSenderService>();

            await timeOffService.SendRegistrationCode("andrey.nikolayev@trinetix.com", "111111");
        }
    }
}
