using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateSkillGetAll
{
    public class CandidateSkillGetAllQuery : IRequest<List<CandidateSkillResponse>>
    {
    }
}
