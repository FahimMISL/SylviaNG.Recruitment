using SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IMajorSubjectSscHscService
    {
        Task<long> CreateAsync(MajorSubjectSscHscCreateRequest request);
        Task UpdateAsync(long majorSubjectSscHscId, MajorSubjectSscHscUpdateRequest request);
        Task DeleteAsync(long majorSubjectSscHscId);
        Task<List<MajorSubjectSscHscResponse>> GetAllAsync();
    }
}
