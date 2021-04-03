using System.Threading.Tasks;
using Bamboozed.Application.Commands.Entities;

namespace Bamboozed.Application.Commands.Interfaces
{
    public interface ICommandHandler<in T> where T : ICommand
    {
        Task<CommandResult> Handle(T command);
    }
}
