using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateCertifications.Commands.CandidateCertificationUpdate
{
    public class CandidateCertificationUpdateCommand : IRequest<Unit>
    {
        public long CandidateCertificationId { get; set; }
        public CandidateCertificationUpdateRequest Request { get; set; }

        public CandidateCertificationUpdateCommand(long candidateCertificationId, CandidateCertificationUpdateRequest request)
        {
            CandidateCertificationId = candidateCertificationId;
            Request = request;
        }
    }
}
