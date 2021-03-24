using System.Threading.Tasks;

namespace Bamboozed.Application.Commands.Interfaces
{
    public interface ICommandBus
    {
        Task<ICommandResult> Handle(ICommand command);
    }
}
