using MediatR;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateTagDelete
{
    public class CandidateTagDeleteCommand : IRequest<Unit>
    {
        public long CandidateProfileId { get; set; }
        public long CandidateTagId { get; set; }

        public CandidateTagDeleteCommand(long candidateProfileId, long candidateTagId)
        {
            CandidateProfileId = candidateProfileId;
            CandidateTagId = candidateTagId;
        }
    }
}
