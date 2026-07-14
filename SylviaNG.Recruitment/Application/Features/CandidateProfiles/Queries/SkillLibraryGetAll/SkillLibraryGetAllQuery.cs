using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.SkillLibraryGetAll
{
    public class SkillLibraryGetAllQuery : IRequest<List<SkillLibraryItemResponse>>
    {
    }
}
