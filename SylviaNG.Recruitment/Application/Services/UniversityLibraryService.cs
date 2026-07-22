using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Services
{
    public class UniversityLibraryService : IUniversityLibraryService
    {
        private readonly IUniversityLibraryItemRepository _universityLibraryItemRepository;

        public UniversityLibraryService(IUniversityLibraryItemRepository universityLibraryItemRepository)
        {
            _universityLibraryItemRepository = universityLibraryItemRepository;
        }

        public async Task<List<UniversityLibraryItemResponse>> GetAllAsync()
        {
            var entities = await _universityLibraryItemRepository.GetAllOrderedAsync();
            return entities.Select(e => new UniversityLibraryItemResponse
            {
                UniversityLibraryItemId = e.UniversityLibraryItemId,
                Name = e.Name,
                Code = e.Code
            }).ToList();
        }
    }
}
