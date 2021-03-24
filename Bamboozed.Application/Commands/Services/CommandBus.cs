using System;
using System.Threading.Tasks;
using Bamboozed.Application.Commands.Interfaces;

namespace Bamboozed.Application.Commands.Services
{
    public class CommandBus : ICommandBus
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<ICommandResult> Handle(ICommand command)
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
            {
                throw new Exception($"Handler for type {command.GetType().Name} is not defined");
            }

            var method = handlerType.GetMethod("Handle");

            if (method == null)
            {
                throw new Exception($"Handle function for command handler of {command.GetType().Name} is not found");
            }

            return (Task<ICommandResult>)method.Invoke(handler, new[] { command });
        }
    }
}
