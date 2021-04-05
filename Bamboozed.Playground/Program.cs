using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Bamboozed.Application;
using Bamboozed.Application.Commands.Entities;
using Bamboozed.Application.Commands.Services;

namespace Bamboozed.Playground
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var collection = new ServiceCollection();
            ApplicationConfiguration.Setup(collection);
            var serviceProvider = collection.BuildServiceProvider();

            var notificationService = serviceProvider.GetRequiredService<CommandBus>();

            await notificationService.Handle(new RegisterCommand
            {
                Email = "adsg@gmail.com",
                ConversationId = "29:10N-S1JIYcRAn3ScGF6wz72lOPSr8iHexFX7IIEuHc_tuTxsqfp7tM08VgZo9V-Sx"
            });
        }
    }
}
