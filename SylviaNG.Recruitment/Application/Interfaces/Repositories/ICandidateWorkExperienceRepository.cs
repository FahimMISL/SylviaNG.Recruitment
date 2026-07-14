using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ICandidateWorkExperienceRepository : IRepository<CandidateWorkExperience>
    {
        Task<List<CandidateWorkExperience>> GetAllByCandidateProfileIdAsync(long candidateProfileId);
    }
}
