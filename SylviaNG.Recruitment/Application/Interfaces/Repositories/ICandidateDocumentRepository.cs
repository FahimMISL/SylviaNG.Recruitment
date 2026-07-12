using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ICandidateDocumentRepository : IRepository<CandidateDocument>
    {
        Task<List<CandidateDocument>> GetAllByCandidateProfileIdAsync(long candidateProfileId);
    }
}
