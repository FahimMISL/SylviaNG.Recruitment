using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IJobApplicationSubmissionService
    {
        /// <summary>
        /// Anonymous/authenticated candidate apply flow used by the career portal (source=External)
        /// and internal job board (source=Internal). Validates the posting is Open and audience-matched,
        /// rejects duplicate (email, jobPostingId) applications, optionally stores the uploaded CV.
        /// </summary>
        Task<JobApplicationResponse> SubmitAsync(JobApplicationSubmitRequest request, ApplicationSourceEnum source);
    }
}
