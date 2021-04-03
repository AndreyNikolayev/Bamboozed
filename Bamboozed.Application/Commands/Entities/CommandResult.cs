using Bamboozed.Application.Commands.Interfaces;

namespace Bamboozed.Application.Commands.Entities
{
    public class CommandResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
