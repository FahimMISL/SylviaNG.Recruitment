using MediatR;
using SylviaNG.Recruitment.Application.Features.VerificationItems.Models;

namespace SylviaNG.Recruitment.Application.Features.VerificationItems.Commands.VerificationItemUpdate
{
    public class VerificationItemUpdateCommand : IRequest<Unit>
    {
        public long VerificationItemId { get; set; }
        public VerificationItemUpdateRequest Request { get; set; }

        public VerificationItemUpdateCommand(long verificationItemId, VerificationItemUpdateRequest request)
        {
            VerificationItemId = verificationItemId;
            Request = request;
        }
    }
}
