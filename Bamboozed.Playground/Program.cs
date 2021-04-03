using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Bamboozed.Application;
using Bamboozed.Application.Services;
using Bamboozed.DAL.Repository;
using Bamboozed.Domain.User;

namespace Bamboozed.Playground
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var collection = new ServiceCollection();
            ApplicationConfiguration.Setup(collection);
            var serviceProvider = collection.BuildServiceProvider();

            var userRepository = serviceProvider.GetRequiredService<IRepository<User>>();

            var a = await userRepository.Get();
        }
    }
}
