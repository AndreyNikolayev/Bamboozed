using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Bamboozed.Application;
using Bamboozed.Application.Interfaces;
using MimeKit;

namespace Bamboozed.Playground
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var collection = new ServiceCollection();
            ApplicationConfiguration.Setup(collection);
            var serviceProvider = collection.BuildServiceProvider();

            var timeOffService = serviceProvider.GetRequiredService<ITimeOffService>();

            await timeOffService.Handle();
        }
    }
}
