using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IJobPostingAttachmentService
    {
        Task<JobPostingAttachmentResponse> UploadAsync(long jobPostingId, IFormFile file);
        Task DeleteAsync(long jobPostingId, long attachmentId);
        Task<List<JobPostingAttachmentResponse>> GetAllByJobPostingIdAsync(long jobPostingId);
    }
}
