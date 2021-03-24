namespace Bamboozed.Application.Commands.Interfaces
{
    public interface ICommandResult
    {
        bool IsSuccess { get; }
        string Message { get; }
    }
}
