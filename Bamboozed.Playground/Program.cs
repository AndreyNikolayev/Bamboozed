using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Bamboozed.Application;
using Bamboozed.Application.Commands.Entities.PolicyCommand;
using Bamboozed.Application.Commands.Interfaces;
using Bamboozed.DAL.Repository;
using Bamboozed.Domain.TimeOffPolicy;
using Bamboozed.Domain.TimeOffRequest;

namespace Bamboozed.Playground
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var collection = new ServiceCollection();
            ApplicationConfiguration.Setup(collection);
            var serviceProvider = collection.BuildServiceProvider();

            var conversationId = Environment.GetEnvironmentVariable("TestConversationId");

            var service = serviceProvider.GetRequiredService<ICommandHandler<ListPolicyCommand>>();

            var a =await service.Handle(new ListPolicyCommand
            {
                ConversationId = conversationId
            });
        }
    }
}
