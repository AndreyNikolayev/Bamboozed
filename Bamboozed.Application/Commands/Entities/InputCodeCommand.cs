using Bamboozed.Application.Commands.Interfaces;
using CommandLine;

namespace Bamboozed.Application.Commands.Entities
{
    [Verb("code", HelpText = "insert code from the registration email")]
    public class InputCodeCommand: ICommand
    {
        [Value(0, Required = true, HelpText = "Code from registration email")]
        public int Code { get; set; }
    }
}
