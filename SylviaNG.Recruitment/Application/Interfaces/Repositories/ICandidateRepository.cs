using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ICandidateRepository : IRepository<Candidate>
    {
        Task<Candidate?> GetByEmailAsync(string email);
        Task<Candidate?> GetByKeycloakUserIdAsync(string keycloakUserId);
    }
}
