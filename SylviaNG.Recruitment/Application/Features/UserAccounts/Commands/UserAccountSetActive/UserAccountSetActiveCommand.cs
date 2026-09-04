using MediatR;

namespace SylviaNG.Recruitment.Application.Features.UserAccounts.Commands.UserAccountSetActive
{
    public class UserAccountSetActiveCommand : IRequest<Unit>
    {
        public long UserAccountId { get; set; }
        public bool IsActive { get; set; }

        public UserAccountSetActiveCommand(long userAccountId, bool isActive)
        {
            UserAccountId = userAccountId;
            IsActive = isActive;
        }
    }
}
