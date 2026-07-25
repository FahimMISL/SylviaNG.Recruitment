using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IDocumentTemplateRepository : IRepository<DocumentTemplate>
    {
        Task<bool> ExistsByCodeAsync(string code, long? excludeId = null);
        Task<List<DocumentTemplate>> GetAllOrderedAsync();
        Task<int> CountOfferLetterUsageAsync(long documentTemplateId);
        Task AddVersionAsync(DocumentTemplateVersion version);
        Task<List<DocumentTemplateVersion>> GetVersionsOrderedAsync(long documentTemplateId);
    }
}
