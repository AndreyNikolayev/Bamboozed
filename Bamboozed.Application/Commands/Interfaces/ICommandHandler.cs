using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Bamboozed.Application.Commands.Interfaces
{
    public interface ICommandHandler<in T> where T : ICommand
    {
        Task<Result> Handle(T command);
    }
}
