using System.Threading.Tasks;

namespace Bamboozed.Application.Commands.Interfaces
{
    public interface ICommandHandler<in T> where T : ICommand
    {
        Task<ICommandResult> Handle(T command);
    }
}
