using SylviaNG.Recruitment.Application.Features.ReferralSources.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IReferralSourceService
    {
        Task<long> CreateAsync(ReferralSourceCreateRequest request);
        Task UpdateAsync(long referralSourceId, ReferralSourceUpdateRequest request);
        Task DeleteAsync(long referralSourceId);
        Task<List<ReferralSourceResponse>> GetAllAsync();
    }
}
