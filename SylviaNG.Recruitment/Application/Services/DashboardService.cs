using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Extensions;
using SylviaNG.Recruitment.Application.Features.Dashboard.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IHiringPipelineRepository _hiringPipelineRepository;
        private readonly ICandidateProfileService _candidateProfileService;

        public DashboardService(
            IHttpContextAccessor httpContextAccessor,
            IJobPostingRepository jobPostingRepository,
            IJobApplicationRepository jobApplicationRepository,
            IHiringPipelineRepository hiringPipelineRepository,
            ICandidateProfileService candidateProfileService)
        {
            _httpContextAccessor = httpContextAccessor;
            _jobPostingRepository = jobPostingRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _hiringPipelineRepository = hiringPipelineRepository;
            _candidateProfileService = candidateProfileService;
        }

        public async Task<DashboardSummaryResponse> GetSummaryAsync()
        {
            var role = GetRole();

            if (role == UserRoleEnum.Candidate)
            {
                var profile = await _candidateProfileService.GetMyProfileAsync();
                return new DashboardSummaryResponse
                {
                    Role = role.ToString(),
                    ProfileCompletenessPercentage = profile.CompletenessPercentage
                };
            }

            var openJobPostingsCount = await _jobPostingRepository.CountByStatusAsync(JobStatusEnum.Open);
            var totalApplicationsCount = await _jobApplicationRepository.CountAllAsync();
            var activeHiringPipelinesCount = await _hiringPipelineRepository.CountActiveAsync();

            return new DashboardSummaryResponse
            {
                Role = role.ToString(),
                OpenJobPostingsCount = openJobPostingsCount,
                TotalApplicationsCount = totalApplicationsCount,
                ActiveHiringPipelinesCount = activeHiringPipelinesCount
            };
        }

        private UserRoleEnum GetRole()
        {
            var user = _httpContextAccessor.HttpContext?.User
                ?? throw new UnauthorizedAccessException("No authenticated user in the current request.");

            return user.GetHighestRole();
        }
    }
}
