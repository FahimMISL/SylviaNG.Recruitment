using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateCertificationUpdate
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
