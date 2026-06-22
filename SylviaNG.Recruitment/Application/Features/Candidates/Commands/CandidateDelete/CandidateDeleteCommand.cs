using MediatR;

namespace SylviaNG.Recruitment.Application.Features.Candidates.Commands.CandidateDelete
{
    public class CandidateDeleteCommand : IRequest<Unit>
    {
        public long CandidateId { get; set; }

        public CandidateDeleteCommand(long candidateId)
        {
            CandidateId = candidateId;
        }
    }
}
