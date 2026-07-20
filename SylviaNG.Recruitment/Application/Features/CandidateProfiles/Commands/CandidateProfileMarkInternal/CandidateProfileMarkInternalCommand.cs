using MediatR;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfileMarkInternal
{
    public class CandidateProfileMarkInternalCommand : IRequest<Unit>
    {
        public long CandidateProfileId { get; set; }

        public CandidateProfileMarkInternalCommand(long candidateProfileId)
        {
            CandidateProfileId = candidateProfileId;
        }
    }
}
