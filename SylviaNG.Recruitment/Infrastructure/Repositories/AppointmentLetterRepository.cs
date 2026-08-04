using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class AppointmentLetterRepository : Repository<AppointmentLetter>, IAppointmentLetterRepository
    {
        public AppointmentLetterRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<AppointmentLetter>> GetAllOrderedAsync(long? jobApplicationId)
        {
            var query = _dbSet
                .Include(a => a.JobApplication)
                .Include(a => a.OfferLetter)
                .Include(a => a.DocumentTemplate)
                .AsQueryable();

            if (jobApplicationId.HasValue)
                query = query.Where(a => a.JobApplicationId == jobApplicationId.Value);

            return await query.OrderByDescending(a => a.GeneratedAt).ToListAsync();
        }

        public async Task<AppointmentLetter?> GetByIdWithDetailsAsync(long appointmentLetterId)
        {
            return await _dbSet
                .Include(a => a.JobApplication)
                .Include(a => a.OfferLetter)
                .Include(a => a.DocumentTemplate)
                .FirstOrDefaultAsync(a => a.AppointmentLetterId == appointmentLetterId);
        }

        public async Task<List<AppointmentLetter>> GetAllForCandidateAsync(long candidateProfileId)
        {
            return await _dbSet
                .Include(a => a.JobApplication)
                .Include(a => a.OfferLetter)
                .Include(a => a.DocumentTemplate)
                .Where(a => a.JobApplication.CandidateProfileId == candidateProfileId)
                .OrderByDescending(a => a.GeneratedAt)
                .ToListAsync();
        }
    }
}
