using Bamboozed.Application.Commands.Entities;
using Bamboozed.Application.Commands.Interfaces;
using System.Threading.Tasks;

namespace Bamboozed.Application.Commands.Handlers
{
    public class InputPasswordCommandHandler: ICommandHandler<InputPasswordCommand>
    {
        public Task<ICommandResult> Handle(InputPasswordCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}
