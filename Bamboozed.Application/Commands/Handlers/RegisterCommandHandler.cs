using System;
using Bamboozed.Application.Commands.Entities;
using Bamboozed.Application.Commands.Interfaces;
using System.Threading.Tasks;
using Bamboozed.Application.Context.Interfaces;
using Bamboozed.Application.Interfaces;
using Bamboozed.DAL.Entities;
using Bamboozed.DAL.Enums;
using Bamboozed.DAL.Repository;
using Newtonsoft.Json;

namespace Bamboozed.Application.Commands.Handlers
{
    public class RegisterCommandHandler: ICommandHandler<RegisterCommand>
    {
        private readonly IReadonlyConversationReferenceContext _conversationReferenceContext;
        private readonly IRepository<User> _userRepository;
        private readonly IMailSenderService _mailSenderService;

        public RegisterCommandHandler(IReadonlyConversationReferenceContext conversationReferenceContext,
            IRepository<User> userRepository,
            IMailSenderService mailSenderService)
        {
            _conversationReferenceContext = conversationReferenceContext;
            _userRepository = userRepository;
            _mailSenderService = mailSenderService;
        }

        public async Task<ICommandResult> Handle(RegisterCommand command)
        {
            var user = await _userRepository.FirstOrDefault(new FilterRequest(rowKey: command.Email));

            if (user != null)
            {
                var result = new CommandResult
                {
                    IsSuccess = false
                };

                if (user.ConversationId != _conversationReferenceContext.Context.User.Id)
                {
                    result.Message =
                        "User is already associated with another chat. Contact admin in order to change chat";
                    return result;
                }

                switch (user.UserStatus)
                {
                    case UserStatus.RegistrationCodeSent:
                        result.Message =
                            "Registration code is already sent to your mailbox. Please submit it with 'code' command";
                        break;
                    case UserStatus.RegistrationCodeSubmitted:
                        result.Message =
                            "Registration code is already submitted. Please submit BambooHR password it with 'password' command";
                        break;
                    case UserStatus.Active:
                        result.Message = "User is already registered";
                        break;
                }

                return result;
            }

            var registrationCode = new Random().Next(0, 1000000).ToString("D6");

            await _mailSenderService.SendRegistrationCode(command.Email, registrationCode);

            var newUser = new User(command.Email, _conversationReferenceContext.Context.User.Id,
                JsonConvert.SerializeObject(_conversationReferenceContext.Context), registrationCode);

            await _userRepository.Add(newUser);

            return new CommandResult
            {
                IsSuccess = true,
                Message = "Registration code is sent to your mailbox. Please submit it with 'code' command"
            };
        }
    }
}
