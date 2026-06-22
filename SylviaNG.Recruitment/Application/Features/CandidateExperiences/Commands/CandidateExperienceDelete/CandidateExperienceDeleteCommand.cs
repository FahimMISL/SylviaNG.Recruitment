using MediatR;

namespace SylviaNG.Recruitment.Application.Features.CandidateExperiences.Commands.CandidateExperienceDelete
{
    public class CandidateExperienceDeleteCommand : IRequest<Unit>
    {
        public long CandidateExperienceId { get; set; }

        public CandidateExperienceDeleteCommand(long candidateExperienceId)
        {
            CandidateExperienceId = candidateExperienceId;
        }
    }
}
