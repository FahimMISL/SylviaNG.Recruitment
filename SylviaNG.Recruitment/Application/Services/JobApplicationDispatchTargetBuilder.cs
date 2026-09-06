using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Services
{
    public class JobApplicationDispatchTargetBuilder : IJobApplicationDispatchTargetBuilder
    {
        private readonly IApplicationSettingService _applicationSettingService;

        public JobApplicationDispatchTargetBuilder(IApplicationSettingService applicationSettingService)
        {
            _applicationSettingService = applicationSettingService;
        }

        public async Task<NotificationDispatchTargets> BuildTargetsAsync(JobApplication entity)
        {
            var hrEmail = await _applicationSettingService.GetHrNotificationEmailAsync();
            // CompanyId passed explicitly - the HR fan-out otherwise leans on the ambient
            // ICompanyScoped filter, which matches every tenant whenever no company scope is set
            // (anonymous career-portal submits, and the background dispatch worker).
            return new NotificationDispatchTargets(
                entity.CandidateEmail,
                hrEmail,
                entity.JobApplicationId,
                NotifyActiveHrUsers: true,
                CompanyId: entity.CompanyId);
        }

        public static Dictionary<string, string> BuildBasePlaceholders(JobApplication entity, string jobPostingTitle)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CandidateName"] = entity.CandidateName,
                ["JobPostingTitle"] = jobPostingTitle,
                ["ApplicationStatus"] = entity.ApplicationStatus.ToString()
            };
        }
    }
}
