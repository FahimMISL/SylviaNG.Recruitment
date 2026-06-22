using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateSkills.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateSkills.Queries.CandidateSkillGetAll
{
    public class CandidateSkillGetAllQuery : IRequest<List<CandidateSkillResponse>>
    {
    }
}
