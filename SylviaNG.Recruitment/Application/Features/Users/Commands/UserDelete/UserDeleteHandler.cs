using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Users.Commands.UserDelete
{
    public class UserDeleteHandler : IRequestHandler<UserDeleteCommand, Unit>
    {
        private readonly IUserService _userService;

        public UserDeleteHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Unit> Handle(UserDeleteCommand command, CancellationToken cancellationToken)
        {
            await _userService.DeleteAsync(command.UserId);
            return Unit.Value;
        }
    }
}
