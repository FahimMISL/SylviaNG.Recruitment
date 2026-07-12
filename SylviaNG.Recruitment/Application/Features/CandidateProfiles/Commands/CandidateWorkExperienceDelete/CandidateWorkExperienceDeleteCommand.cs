using MediatR;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateWorkExperienceDelete
{
    public class CandidateWorkExperienceDeleteCommand : IRequest<Unit>
    {
        public long CandidateWorkExperienceId { get; set; }

        public CandidateWorkExperienceDeleteCommand(long candidateWorkExperienceId)
        {
            CandidateWorkExperienceId = candidateWorkExperienceId;
        }
    }
}
