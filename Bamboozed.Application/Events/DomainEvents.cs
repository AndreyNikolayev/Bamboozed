using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bamboozed.Application.Events.Interfaces;

namespace Bamboozed.Application.Events
{
    public static class DomainEvents
    {
        private static List<Type> _handlers;

        public static void Init()
        {
            _handlers = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
                .ToList();
        }

        public static async Task Dispatch(IDomainEvent domainEvent)
        {
            foreach (var handlerType in _handlers)
            {
                var canHandleEvent = handlerType.GetInterfaces()
                    .Any(x => x.IsGenericType
                              && x.GetGenericTypeDefinition() == typeof(IEventHandler<>)
                              && x.GenericTypeArguments[0] == domainEvent.GetType());

                if (canHandleEvent)
                {
                    dynamic handler = Activator.CreateInstance(handlerType);
                    await handler.HandleAsync((dynamic)domainEvent);
                }
            }
        }
    }
}
