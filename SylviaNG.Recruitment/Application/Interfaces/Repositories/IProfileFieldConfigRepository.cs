using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IProfileFieldConfigRepository : IRepository<ProfileFieldConfig>
    {
        Task<bool> ExistsAsync(CandidateProfileFieldEnum field, long? jobPostingId, long? excludeId = null);
        Task<List<ProfileFieldConfig>> GetGlobalAndForJobPostingAsync(long? jobPostingId);
    }
}
