using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Bamboozed.Application;
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

            var commandParser = serviceProvider.GetRequiredService<CommandParser>();

            var lol = commandParser.GetCommand("help");
        }
    }
}
