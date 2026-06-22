using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Services;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public JobApplicationService(
            IJobApplicationRepository jobApplicationRepository,
            IJobPostingRepository jobPostingRepository,
            IUnitOfWork unitOfWork,
            IEmailService emailService)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _jobPostingRepository = jobPostingRepository;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<long> CreateAsync(JobApplicationCreateRequest request)
        {
            var jobPosting = await _jobPostingRepository.GetByIdAsync(request.JobPostingId)
                ?? throw new NotFoundException("JobPosting", request.JobPostingId);

            if (!string.IsNullOrEmpty(request.CandidateEmail))
            {
                var exists = await _jobApplicationRepository.GetByEmailAndJobPostingIdAsync(request.CandidateEmail, request.JobPostingId);
                if (exists != null)
                    throw new DuplicateException("JobApplication", "CandidateEmail", request.CandidateEmail);
            }

            var entity = request.ToEntity();
            entity.ApplicationStatus = ApplicationStatusEnum.Applied;
            entity.AppliedDate = DateTime.UtcNow;

            await _jobApplicationRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.JobApplicationId;
        }

        public async Task UpdateAsync(long jobApplicationId, JobApplicationUpdateRequest request)
        {
            var entity = await _jobApplicationRepository.GetByIdAsync(jobApplicationId)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            var oldStatus = entity.ApplicationStatus;
            entity.ApplyUpdate(request);
            _jobApplicationRepository.Update(entity);

            if (request.ApplicationStatus.HasValue && request.ApplicationStatus.Value != oldStatus && entity.CandidateId.HasValue)
            {
                var candidate = await _unitOfWork.Context.Set<Candidate>()
                    .FirstOrDefaultAsync(c => c.CandidateId == entity.CandidateId.Value);

                var jobTitle = await _unitOfWork.Context.Set<JobPosting>()
                    .Where(j => j.JobPostingId == entity.JobPostingId)
                    .Select(j => j.Title)
                    .FirstOrDefaultAsync() ?? "a position";

                var newStatus = request.ApplicationStatus.Value;
                var statusText = FormatStatus(newStatus);

                if (candidate != null && !string.IsNullOrEmpty(candidate.KeycloakUserId))
                {
                    var notification = new UserNotification
                    {
                        KeycloakUserId = candidate.KeycloakUserId,
                        Title = "Application Status Updated",
                        Message = $"Your application for \"{jobTitle}\" has been updated to: {statusText}.",
                        NotificationType = newStatus == ApplicationStatusEnum.Rejected
                            ? UserNotificationTypeEnum.Warning
                            : newStatus == ApplicationStatusEnum.Hired
                                ? UserNotificationTypeEnum.Success
                                : UserNotificationTypeEnum.Info,
                        ActionUrl = "/my-applications",
                        IsActive = true
                    };
                    await _unitOfWork.Context.Set<UserNotification>().AddAsync(notification);

                    if (!string.IsNullOrEmpty(candidate.Email))
                    {
                        var candidateName = candidate.FullName ?? "Candidate";
                        var candidateEmail = candidate.Email;
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await _emailService.SendAsync(
                                    candidateEmail, candidateName,
                                    $"Application Update — {jobTitle}",
                                    EmailTemplates.ApplicationStatusUpdate(candidateName, jobTitle, statusText));
                            }
                            catch { }
                        });
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private static string FormatStatus(ApplicationStatusEnum status) => status switch
        {
            ApplicationStatusEnum.Applied => "Applied",
            ApplicationStatusEnum.Screening => "Under Screening",
            ApplicationStatusEnum.Shortlisted => "Shortlisted",
            ApplicationStatusEnum.InterviewScheduled => "Interview Scheduled",
            ApplicationStatusEnum.Interviewed => "Interviewed",
            ApplicationStatusEnum.Offered => "Offer Extended",
            ApplicationStatusEnum.Hired => "Hired",
            ApplicationStatusEnum.Rejected => "Rejected",
            ApplicationStatusEnum.Withdrawn => "Withdrawn",
            _ => status.ToString()
        };

        public async Task DeleteAsync(long jobApplicationId)
        {
            var entity = await _jobApplicationRepository.GetByIdAsync(jobApplicationId)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            _jobApplicationRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<JobApplicationResponse> GetByIdAsync(long jobApplicationId)
        {
            var entity = await _jobApplicationRepository.GetByIdWithIncludeAsync(
                a => a.JobApplicationId == jobApplicationId,
                a => a.JobPosting)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<JobApplicationResponse>> GetPaginatedByJobPostingAsync(long jobPostingId, PagedRequest request)
        {
            var pagedResult = await _jobApplicationRepository.GetPaginatedByJobPostingAsync(jobPostingId, request);

            return new PagedResult<JobApplicationResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<PagedResult<JobApplicationResponse>> GetAllPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _jobApplicationRepository.GetAllPaginatedAsync(request);

            return new PagedResult<JobApplicationResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
