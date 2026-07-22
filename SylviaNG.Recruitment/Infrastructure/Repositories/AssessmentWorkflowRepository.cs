using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class AssessmentWorkflowRepository : Repository<AssessmentWorkflow>, IAssessmentWorkflowRepository
    {
        public AssessmentWorkflowRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(w => w.Name == name && (!excludeId.HasValue || w.AssessmentWorkflowId != excludeId.Value));
        }

        public async Task<AssessmentWorkflow?> GetByIdWithStagesAsync(long assessmentWorkflowId)
        {
            return await _dbSet
                .Include(w => w.Stages.OrderBy(s => s.DisplayOrder))
                .Include(w => w.JobPostings)
                .FirstOrDefaultAsync(w => w.AssessmentWorkflowId == assessmentWorkflowId);
        }

        public async Task<List<AssessmentWorkflow>> GetAllWithStagesAsync()
        {
            return await _dbSet
                .Include(w => w.Stages.OrderBy(s => s.DisplayOrder))
                .Include(w => w.JobPostings)
                .ToListAsync();
        }

        public async Task<List<AssessmentWorkflow>> GetActiveAsync()
        {
            return await _dbSet
                .Where(w => w.IsActive)
                .OrderBy(w => w.Name)
                .ToListAsync();
        }
    }
}
