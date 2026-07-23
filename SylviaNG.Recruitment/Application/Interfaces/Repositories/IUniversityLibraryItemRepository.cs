using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IUniversityLibraryItemRepository : IRepository<UniversityLibraryItem>
    {
        Task<List<UniversityLibraryItem>> GetAllOrderedAsync();
    }
}
