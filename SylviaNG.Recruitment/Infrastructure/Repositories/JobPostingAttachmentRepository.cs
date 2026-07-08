using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class JobPostingAttachmentRepository : Repository<JobPostingAttachment>, IJobPostingAttachmentRepository
    {
        public JobPostingAttachmentRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<JobPostingAttachment>> GetAllByJobPostingIdAsync(long jobPostingId)
        {
            return await _dbSet
                .Where(a => a.JobPostingId == jobPostingId)
                .ToListAsync();
        }
    }
}
