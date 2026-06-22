using MediatR;

namespace SylviaNG.Recruitment.Application.Features.VerificationItems.Commands.VerificationItemDelete
{
    public class VerificationItemDeleteCommand : IRequest<Unit>
    {
        public long VerificationItemId { get; set; }

        public VerificationItemDeleteCommand(long verificationItemId)
        {
            VerificationItemId = verificationItemId;
        }
    }
}
