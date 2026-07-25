using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class MedicalLetterRepository : Repository<MedicalLetter>, IMedicalLetterRepository
    {
        public MedicalLetterRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<MedicalLetter>> GetAllOrderedAsync(long? jobApplicationId)
        {
            var query = _dbSet
                .Include(m => m.JobApplication)
                .Include(m => m.OfferLetter)
                .Include(m => m.DocumentTemplate)
                .AsQueryable();

            if (jobApplicationId.HasValue)
                query = query.Where(m => m.JobApplicationId == jobApplicationId.Value);

            return await query.OrderByDescending(m => m.GeneratedAt).ToListAsync();
        }

        public async Task<MedicalLetter?> GetByIdWithDetailsAsync(long medicalLetterId)
        {
            return await _dbSet
                .Include(m => m.JobApplication)
                .Include(m => m.OfferLetter)
                .Include(m => m.DocumentTemplate)
                .FirstOrDefaultAsync(m => m.MedicalLetterId == medicalLetterId);
        }
    }
}
