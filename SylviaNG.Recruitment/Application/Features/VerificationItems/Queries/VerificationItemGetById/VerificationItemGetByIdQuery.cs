using MediatR;
using SylviaNG.Recruitment.Application.Features.VerificationItems.Models;

namespace SylviaNG.Recruitment.Application.Features.VerificationItems.Queries.VerificationItemGetById
{
    public class VerificationItemGetByIdQuery : IRequest<VerificationItemResponse>
    {
        public long VerificationItemId { get; set; }

        public VerificationItemGetByIdQuery(long verificationItemId)
        {
            VerificationItemId = verificationItemId;
        }
    }
}
