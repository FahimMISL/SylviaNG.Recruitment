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
            return await _dbSet.OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<int> CountOfferLetterUsageAsync(long documentTemplateId)
        {
            return await _dbContext.OfferLetters.CountAsync(o => o.DocumentTemplateId == documentTemplateId);
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
