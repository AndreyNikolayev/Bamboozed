﻿using System.Linq;
using System.Threading.Tasks;
using Bamboozed.Application.Commands.Interfaces;
using Bamboozed.Application.Context;
using Bamboozed.Application.Events;
using Bamboozed.Application.Events.Events;
using Bamboozed.DAL.Repository;
using Bamboozed.Domain.Extensions;
using Bamboozed.Domain.User;
using CommandLine;
using CSharpFunctionalExtensions;

namespace Bamboozed.Application.Commands.Entities
{
    [Verb("code", HelpText = "insert code from the registration email")]
    public class InputCodeCommand: ICommand
    {
        [Value(0, Required = true, HelpText = "Code from registration email")]
        public string Code { get; set; }
    }

    public class InputCodeCommandHandler : ICommandHandler<InputCodeCommand>
    {
        private readonly ReadonlyConversationReferenceContext _conversationReferenceContext;
        private readonly IRepository<User> _userRepository;

        public InputCodeCommandHandler(
            ReadonlyConversationReferenceContext conversationReferenceContext,
            IRepository<User> userRepository)
        {
            _conversationReferenceContext = conversationReferenceContext;
            _userRepository = userRepository;
        }

        public async Task<Result> Handle(InputCodeCommand command)
        {
            return await _userRepository.Get()
                .ToResultTask()
                .Map(users => users
                    .FirstOrDefault(p => p.ConversationId == _conversationReferenceContext.Context.User.Id)
                )
                .Ensure(user => user != null, "Chat is not recognized. Please use 'register' command first.")
                .Check(user => user.SubmitRegistrationCode(command.Code))
                .Tap(user => _userRepository.Edit(user))
                .Tap(user => DomainEvents.Dispatch(new RegistrationCodeEnteredEvent(user.Email)))
                .OnFailure(error => DomainEvents.Dispatch(new RegistrationStepFailedEvent(_conversationReferenceContext.Context, error)))
                .Bind(_ => Result.Success());
        }
    }
}
