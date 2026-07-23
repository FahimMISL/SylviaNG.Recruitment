using SylviaNG.Recruitment.Application.Features.Scorecards.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IScorecardService
    {
        Task<long> CreateAsync(ScorecardCreateRequest request);
        Task UpdateAsync(long scorecardId, ScorecardUpdateRequest request);
        Task SetActiveStatusAsync(long scorecardId, bool isActive);
        Task<ScorecardResponse> GetByIdAsync(long scorecardId);
        Task<List<ScorecardResponse>> GetAllAsync();
        Task<List<ScorecardLookupResponse>> GetActiveLookupAsync();
    }
}
