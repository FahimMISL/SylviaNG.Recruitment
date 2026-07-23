using SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IMajorSubjectUniversityService
    {
        Task<long> CreateAsync(MajorSubjectUniversityCreateRequest request);
        Task UpdateAsync(long majorSubjectUniversityId, MajorSubjectUniversityUpdateRequest request);
        Task DeleteAsync(long majorSubjectUniversityId);
        Task<List<MajorSubjectUniversityResponse>> GetAllAsync();
    }
}
