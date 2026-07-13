using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IJobPostingAttachmentRepository : IRepository<JobPostingAttachment>
    {
        Task<List<JobPostingAttachment>> GetAllByJobPostingIdAsync(long jobPostingId);
    }
}
