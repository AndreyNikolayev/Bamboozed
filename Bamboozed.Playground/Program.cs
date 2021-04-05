using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Bamboozed.Application;
using Bamboozed.Application.Services;
using Bamboozed.Domain;
using Microsoft.Bot.Schema;

namespace Bamboozed.Playground
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var collection = new ServiceCollection();
            ApplicationConfiguration.Setup(collection);
            var serviceProvider = collection.BuildServiceProvider();

            var notificationService = serviceProvider.GetRequiredService<NotificationService>();

            var conversationReference = new ConversationReference(
               // user: new ChannelAccount("29:10N-S1JIYcRAn3ScGF6wz72lOPSr8iHexFX7IIEuHc_tuTxsqfp7tM08VgZo9V-Sx"),
               // bot: new ChannelAccount("28:129fa032-ec89-4f2d-b23b-88b852be9030"),
                conversation: new ConversationAccount(id: "29:10N-S1JIYcRAn3ScGF6wz72lOPSr8iHexFX7IIEuHc_tuTxsqfp7tM08VgZo9V-Sx"),
                serviceUrl: "https://smba.trafficmanager.net/apis/"
              //  channelId:"skype"
            );

            await notificationService.Notify(new NotificationRequest(conversationReference.Conversation.Id, "lol"));
        }
    }
}
