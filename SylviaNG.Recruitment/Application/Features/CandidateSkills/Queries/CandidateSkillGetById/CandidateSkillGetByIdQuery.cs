using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateSkills.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateSkills.Queries.CandidateSkillGetById
{
    public class CandidateSkillGetByIdQuery : IRequest<CandidateSkillResponse>
    {
        public long CandidateSkillId { get; set; }

        public CandidateSkillGetByIdQuery(long candidateSkillId)
        {
            CandidateSkillId = candidateSkillId;
        }
    }
}
