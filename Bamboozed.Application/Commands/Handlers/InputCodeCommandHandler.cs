using Bamboozed.Application.Commands.Entities;
using Bamboozed.Application.Commands.Interfaces;
using System.Threading.Tasks;

namespace Bamboozed.Application.Commands.Handlers
{
    public class InputCodeCommandHandler: ICommandHandler<InputCodeCommand>
    {
        public Task<ICommandResult> Handle(InputCodeCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}
