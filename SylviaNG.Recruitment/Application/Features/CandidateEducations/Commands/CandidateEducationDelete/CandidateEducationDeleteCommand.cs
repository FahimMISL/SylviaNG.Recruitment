using MediatR;

namespace SylviaNG.Recruitment.Application.Features.CandidateEducations.Commands.CandidateEducationDelete
{
    public class CandidateEducationDeleteCommand : IRequest<Unit>
    {
        public long CandidateEducationId { get; set; }

        public CandidateEducationDeleteCommand(long candidateEducationId)
        {
            CandidateEducationId = candidateEducationId;
        }
    }
}
