using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateExperiences.Queries.CandidateExperienceGetById
{
    public class CandidateExperienceGetByIdQuery : IRequest<CandidateExperienceResponse>
    {
        public long CandidateExperienceId { get; set; }

        public CandidateExperienceGetByIdQuery(long candidateExperienceId)
        {
            CandidateExperienceId = candidateExperienceId;
        }
    }
}
