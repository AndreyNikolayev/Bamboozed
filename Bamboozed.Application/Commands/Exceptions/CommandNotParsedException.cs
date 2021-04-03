using System;

namespace Bamboozed.Application.Commands.Exceptions
{
    public class CommandNotParsedException: Exception
    {
        public CommandNotParsedException(string message) : base(message)
        {
        }
    }
}
