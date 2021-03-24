using Bamboozed.Application.Commands.Interfaces;
using CommandLine;

namespace Bamboozed.Application.Commands.Entities
{
    [Verb("register", HelpText = "register email")]
    public class RegisterCommand : ICommand
    {
        [Value(0, Required = true, HelpText = "Your Bamboo Email")]
        public string Email { get; set; }
    }
}
