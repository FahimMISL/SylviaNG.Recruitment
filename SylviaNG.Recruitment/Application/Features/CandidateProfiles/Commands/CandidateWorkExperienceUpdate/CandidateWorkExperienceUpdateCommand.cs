using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateWorkExperienceUpdate
{
    public class CandidateWorkExperienceUpdateCommand : IRequest<Unit>
    {
        public long CandidateWorkExperienceId { get; set; }
        public CandidateWorkExperienceUpdateRequest Request { get; set; }

        public CandidateWorkExperienceUpdateCommand(long candidateWorkExperienceId, CandidateWorkExperienceUpdateRequest request)
        {
            CandidateWorkExperienceId = candidateWorkExperienceId;
            Request = request;
        }
    }
}
