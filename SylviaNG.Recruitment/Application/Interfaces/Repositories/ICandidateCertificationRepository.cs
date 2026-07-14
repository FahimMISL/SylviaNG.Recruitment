using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ICandidateCertificationRepository : IRepository<CandidateCertification>
    {
        Task<List<CandidateCertification>> GetAllByCandidateProfileIdAsync(long candidateProfileId);
    }
}
