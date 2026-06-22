using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateExperiences.Queries.CandidateExperienceGetAll
{
    public class CandidateExperienceGetAllQuery : IRequest<List<CandidateExperienceResponse>>
    {
    }
}
