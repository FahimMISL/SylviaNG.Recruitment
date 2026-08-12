using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class DocumentTemplateRepository : Repository<DocumentTemplate>, IDocumentTemplateRepository
    {
        public DocumentTemplateRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByCodeAsync(string code, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(t => t.Code == code && (!excludeId.HasValue || t.DocumentTemplateId != excludeId.Value));
        }

        public async Task<List<DocumentTemplate>> GetAllOrderedAsync()
        {
            // Newest-first: a freshly created/edited template surfaces at the top of the admin list.
            return await _dbSet.OrderByDescending(t => t.DocumentTemplateId).ToListAsync();
        }

        public async Task<int> CountUsageAsync(long documentTemplateId)
        {
            var offerLetterCount = await _dbContext.OfferLetters.CountAsync(o => o.DocumentTemplateId == documentTemplateId);
            var appointmentLetterCount = await _dbContext.AppointmentLetters.CountAsync(a => a.DocumentTemplateId == documentTemplateId);
            var joiningBookletCount = await _dbContext.JoiningBooklets.CountAsync(j => j.DocumentTemplateId == documentTemplateId);
            var medicalLetterCount = await _dbContext.MedicalLetters.CountAsync(m => m.DocumentTemplateId == documentTemplateId);
            var targetLetterCount = await _dbContext.TargetLetters.CountAsync(t => t.DocumentTemplateId == documentTemplateId);

            return offerLetterCount + appointmentLetterCount + joiningBookletCount + medicalLetterCount + targetLetterCount;
        }

        public async Task AddVersionAsync(DocumentTemplateVersion version)
        {
            await _dbContext.DocumentTemplateVersions.AddAsync(version);
        }

        public async Task<List<DocumentTemplateVersion>> GetVersionsOrderedAsync(long documentTemplateId)
        {
            return await _dbContext.DocumentTemplateVersions
                .Where(v => v.DocumentTemplateId == documentTemplateId)
                .OrderByDescending(v => v.VersionNumber)
                .ToListAsync();
        }
    }
}
