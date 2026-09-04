using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IProfileFieldConfigService
    {
        Task<long> CreateAsync(ProfileFieldConfigRequest request);
        Task UpdateAsync(long profileFieldConfigId, ProfileFieldConfigRequest request);
        Task DeleteAsync(long profileFieldConfigId);
        Task<List<ProfileFieldConfigResponse>> GetAllAsync();

        /// <summary>Merged view for rendering an apply/profile form: every configurable field with
        /// its effective visibility (job-posting override if one exists, else the global default,
        /// else Optional). jobPostingId null = global-only (e.g. the standalone profile page).</summary>
        Task<List<EffectiveProfileFieldResponse>> GetEffectiveConfigAsync(long? jobPostingId);
    }
}
