using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IUniversityLibraryService
    {
        Task<List<UniversityLibraryItemResponse>> GetAllAsync();
        Task<long> CreateAsync(UniversityLibraryItemCreateRequest request);
        Task UpdateAsync(long universityLibraryItemId, UniversityLibraryItemUpdateRequest request);
        Task DeleteAsync(long universityLibraryItemId);
    }
}
