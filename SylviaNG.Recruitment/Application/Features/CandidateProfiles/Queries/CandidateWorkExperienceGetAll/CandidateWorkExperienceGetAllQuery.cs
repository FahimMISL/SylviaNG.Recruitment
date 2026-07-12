using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateWorkExperienceGetAll
{
    public class CandidateWorkExperienceGetAllQuery : IRequest<List<CandidateWorkExperienceResponse>>
    {
    }
}
