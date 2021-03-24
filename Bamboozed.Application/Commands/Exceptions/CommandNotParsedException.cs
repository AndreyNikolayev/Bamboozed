using System;
using System.Collections.Generic;
using System.Text;

namespace Bamboozed.Application.Commands.Exceptions
{
    public class CommandNotParsedException: Exception
    {
        public CommandNotParsedException(string message) : base(message)
        {
        }
    }
}
