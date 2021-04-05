using System;
using System.Linq;
using Bamboozed.Application.Commands.Interfaces;
using Bamboozed.Application.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Bamboozed.Application
{
    public static class HandlerRegistration
    {
        public static void AddHandlers(this IServiceCollection services)
        {
            var handlerTypes = typeof(ICommand).Assembly.GetTypes()
                .Where(x => x.GetInterfaces().Any(IsHandlerInterface))
                .Where(x => x.Name.EndsWith("Handler"))
                .ToList();

            foreach (var type in handlerTypes)
            {
                AddHandler(services, type);
            }
        }

        private static void AddHandler(IServiceCollection services, Type type)
        {
            var interfaceType = type.GetInterfaces().Single(IsHandlerInterface);

            services.AddScoped(interfaceType, type);
        }

        private static bool IsHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            var typeDefinition = type.GetGenericTypeDefinition();

            return typeDefinition == typeof(ICommandHandler<>) || typeDefinition == typeof(IEventHandler<>);
        }
    }
}
