using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.SkillLibraryGetAll
{
    public class SkillLibraryGetAllHandler : IRequestHandler<SkillLibraryGetAllQuery, List<SkillLibraryItemResponse>>
    {
        private readonly ISkillLibraryService _skillLibraryService;

        public SkillLibraryGetAllHandler(ISkillLibraryService skillLibraryService)
        {
            _skillLibraryService = skillLibraryService;
        }

        public async Task<List<SkillLibraryItemResponse>> Handle(SkillLibraryGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _skillLibraryService.GetAllAsync();
        }
    }
}
