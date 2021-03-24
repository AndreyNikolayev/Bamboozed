using Bamboozed.Application.Commands.Interfaces;
using CommandLine;

namespace Bamboozed.Application.Commands.Entities
{
    [Verb("password", HelpText = "add or update bamboo password")]
    public class InputPasswordCommand : ICommand
    {
        [Value(0, Required = true, HelpText = "Your Bamboo password")]
        public string Password { get; set; }
    }
}
