using System;
using System.Threading.Tasks;
using Bamboozed.Application.Commands.Interfaces;
using Bamboozed.Application.Events.Interfaces;
using CSharpFunctionalExtensions;

namespace Bamboozed.Application.Commands.Services
{
    public class CommandBus
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<Result> Handle(ICommand command)
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

            return (Task<Result>)method.Invoke(handler, new[] { command });
        }
    }
}
