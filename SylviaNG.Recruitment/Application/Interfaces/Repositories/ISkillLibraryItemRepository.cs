using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ISkillLibraryItemRepository : IRepository<SkillLibraryItem>
    {
        Task<List<SkillLibraryItem>> GetAllOrderedAsync();
    }
}
