using SylviaNG.Recruitment.Application.Features.ApplicationSettings.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IApplicationSettingService
    {
        Task<ApplicationSettingResponse> GetAsync();

        /// <summary>Admin-only update (US-007 AC4). Throws FluentValidation.ValidationException
        /// if the percentage is outside 0-100.</summary>
        Task UpdateAsync(ApplicationSettingUpdateRequest request);

        /// <summary>Raw threshold for the JobApplicationService submit gate - 0 means disabled.</summary>
        Task<int> GetMinimumProfileCompletenessPercentageAsync();
    }
}
