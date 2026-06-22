using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Users.Commands.UserUpdate
{
    public class UserUpdateHandler : IRequestHandler<UserUpdateCommand, Unit>
    {
        private readonly IUserService _userService;

        public UserUpdateHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Unit> Handle(UserUpdateCommand command, CancellationToken cancellationToken)
        {
            await _userService.UpdateAsync(command.UserId, command.Request);
            return Unit.Value;
        }
    }
}
