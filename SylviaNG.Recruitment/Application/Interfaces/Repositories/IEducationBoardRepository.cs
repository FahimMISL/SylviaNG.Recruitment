using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IEducationBoardRepository : IRepository<EducationBoard>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<List<EducationBoard>> GetAllOrderedAsync();
        Task<int> CountUsageAsync(long educationBoardId);
    }
}
