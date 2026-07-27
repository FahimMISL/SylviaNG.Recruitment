using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class FinalSelectionPoolRepository : Repository<FinalSelectionPool>, IFinalSelectionPoolRepository
    {
        public FinalSelectionPoolRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<FinalSelectionPool>> GetAllOrderedAsync()
        {
            return await _dbSet
                .Include(p => p.JobApplication)
                .Include(p => p.OfferLetter)
                .Include(p => p.PreBoardingSubmission)
                .OrderByDescending(p => p.EnteredPoolAt)
                .ToListAsync();
        }

        public async Task<FinalSelectionPool?> GetByIdWithDetailsAsync(long finalSelectionPoolId)
        {
            return await _dbSet
                .Include(p => p.JobApplication)
                .Include(p => p.OfferLetter)
                .Include(p => p.PreBoardingSubmission)
                .FirstOrDefaultAsync(p => p.FinalSelectionPoolId == finalSelectionPoolId);
        }

        public async Task<FinalSelectionPool?> GetByOfferLetterIdAsync(long offerLetterId)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.OfferLetterId == offerLetterId);
        }

        public async Task<FinalSelectionPool?> GetByCandidateProfileIdWithDetailsAsync(long candidateProfileId)
        {
            return await _dbSet
                .Include(p => p.JobApplication)
                .Include(p => p.OfferLetter)
                .Include(p => p.PreBoardingSubmission)
                    .ThenInclude(s => s!.Nominees)
                .FirstOrDefaultAsync(p => p.JobApplication.CandidateProfileId == candidateProfileId);
        }
    }
}
