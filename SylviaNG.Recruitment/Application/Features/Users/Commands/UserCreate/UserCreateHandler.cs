using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Users.Commands.UserCreate
{
    public class UserCreateHandler : IRequestHandler<UserCreateCommand, long>
    {
        private readonly IUserService _userService;

        public UserCreateHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<long> Handle(UserCreateCommand command, CancellationToken cancellationToken)
        {
            return await _userService.CreateAsync(command.Request);
        }
    }
}
