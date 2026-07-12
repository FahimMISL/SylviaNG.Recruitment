using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;

namespace SylviaNG.Recruitment.Application.Services
{
    public class SkillLibraryService : ISkillLibraryService
    {
        private readonly ISkillLibraryItemRepository _skillLibraryItemRepository;

        public SkillLibraryService(ISkillLibraryItemRepository skillLibraryItemRepository)
        {
            _skillLibraryItemRepository = skillLibraryItemRepository;
        }

        public async Task<List<SkillLibraryItemResponse>> GetAllAsync()
        {
            var entities = await _skillLibraryItemRepository.GetAllOrderedAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
