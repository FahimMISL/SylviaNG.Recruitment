using MediatR;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateCertificationDelete
{
    public class CandidateCertificationDeleteCommand : IRequest<Unit>
    {
        public long CandidateCertificationId { get; set; }

        public CandidateCertificationDeleteCommand(long candidateCertificationId)
        {
            CandidateCertificationId = candidateCertificationId;
        }
    }
}
