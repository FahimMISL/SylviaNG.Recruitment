using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IJoiningBookletRepository : IRepository<JoiningBooklet>
    {
        Task<List<JoiningBooklet>> GetAllOrderedAsync(long? jobApplicationId);
        Task<JoiningBooklet?> GetByIdWithDetailsAsync(long joiningBookletId);
        Task<List<JoiningBooklet>> GetByIdsWithDetailsAsync(List<long> joiningBookletIds);
    }
}
