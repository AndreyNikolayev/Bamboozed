using System.Linq;
using Bamboozed.Application.Commands.Entities;
using Bamboozed.Application.Commands.Interfaces;
using System.Threading.Tasks;
using Bamboozed.Application.Context.Interfaces;
using Bamboozed.Application.Interfaces;
using Bamboozed.DAL.Entities;
using Bamboozed.DAL.Enums;
using Bamboozed.DAL.Repository;

namespace Bamboozed.Application.Commands.Handlers
{
    public class InputPasswordCommandHandler : ICommandHandler<InputPasswordCommand>
    {
        private readonly IReadonlyConversationReferenceContext _conversationReferenceContext;
        private readonly IRepository<User> _userRepository;
        private readonly IPasswordService _passwordService;

        public InputPasswordCommandHandler(
            IReadonlyConversationReferenceContext conversationReferenceContext,
            IRepository<User> userRepository,
            IPasswordService passwordService)
        {
            _conversationReferenceContext = conversationReferenceContext;
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        public async Task<ICommandResult> Handle(InputPasswordCommand command)
        {
            var userConversationId = _conversationReferenceContext.Context.User.Id;

            var users = await _userRepository.Get();

            var user = users.FirstOrDefault(p => p.ConversationId == userConversationId);

            if (user == null)
            {
                return new CommandResult
                {
                    IsSuccess = false,
                    Message = "Chat is not recognized. Please use 'register' command first."
                };
            }

            if (user.UserStatus == UserStatus.RegistrationCodeSent)
            {
                return new CommandResult
                {
                    IsSuccess = false,
                    Message = "Registration code is sent to your mailbox. Please submit it with 'code' command before submitting the password."
                };
            }

            await _passwordService.Set(user.Email, command.Password);

            if (user.UserStatus == UserStatus.Active)
            {
                return new CommandResult
                {
                    IsSuccess = true,
                    Message = "Your password is changed successfully."
                };
            }

            user.UserStatus = UserStatus.Active;
            await _userRepository.Edit(user);


            return new CommandResult
            {
                IsSuccess = true,
                Message = "Your password is submitted successfully."
            };
        }
    }
}
