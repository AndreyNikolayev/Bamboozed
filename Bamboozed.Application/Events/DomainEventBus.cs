using System;
using System.Threading.Tasks;
using Bamboozed.Application.Events.Interfaces;

namespace Bamboozed.Application.Events
{
    public class DomainEventBus
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task Dispatch(IDomainEvent domainEvent)
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());

            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
            {
                throw new Exception($"Handler for type {domainEvent.GetType().Name} is not defined");
            }

            var method = handlerType.GetMethod("Handle");

            if (method == null)
            {
                throw new Exception($"Handle function for command handler of {domainEvent.GetType().Name} is not found");
            }

            return (Task)method.Invoke(handler, new[] { domainEvent });
        }
    }
}
