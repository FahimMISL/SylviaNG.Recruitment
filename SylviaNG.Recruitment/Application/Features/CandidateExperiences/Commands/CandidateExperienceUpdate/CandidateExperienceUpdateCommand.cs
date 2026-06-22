using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateExperiences.Commands.CandidateExperienceUpdate
{
    public class CandidateExperienceUpdateCommand : IRequest<Unit>
    {
        public long CandidateExperienceId { get; set; }
        public CandidateExperienceUpdateRequest Request { get; set; }

        public CandidateExperienceUpdateCommand(long candidateExperienceId, CandidateExperienceUpdateRequest request)
        {
            CandidateExperienceId = candidateExperienceId;
            Request = request;
        }
    }
}
