using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Bamboozed.Application;
using Bamboozed.AzureFunctions;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Bamboozed.AzureFunctions
{
    public class Startup: FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            ApplicationConfiguration.Setup(builder.Services);
        }
    }
}
