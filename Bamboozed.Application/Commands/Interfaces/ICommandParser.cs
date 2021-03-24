namespace Bamboozed.Application.Commands.Interfaces
{
    public interface ICommandParser
    {
        ICommand GetCommand(string commandText);
    }
}
