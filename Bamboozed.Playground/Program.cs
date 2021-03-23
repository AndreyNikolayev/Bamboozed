using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Bamboozed.Application;

namespace Bamboozed.Playground
{
    class Program
    {
        private const string TimeOffLabel = "Bamboo-TimeOff";
        private const string Login = "bamboozedredirect@gmail.com";
        private const string Password = "ECaZJk2p42XS5Mcs";
        private const string ImapHost = "imap.gmail.com";
        private const int Port = 993;


        private static async Task Main(string[] args)
        {
            //var collection = new ServiceCollection();
            //ApplicationConfiguration.Setup(collection);
            //var serviceProvider = collection.BuildServiceProvider();

            using var client = new ImapClient();

            client.Connect(ImapHost, Port, true);

            client.Authenticate(Login, Password);

            var folder = await client.GetFolderAsync(TimeOffLabel);
            await folder.OpenAsync(FolderAccess.ReadWrite);

            var unreadMessageIdList = await folder.SearchAsync(SearchQuery.NotSeen);

            foreach (var messageId in unreadMessageIdList)
            {
                var message = await folder.GetMessageAsync(messageId);
                Console.WriteLine(message.HtmlBody);

                await folder.AddFlagsAsync(messageId, MessageFlags.Seen, false);
            }

            client.Disconnect(true);
        }
    }
}
