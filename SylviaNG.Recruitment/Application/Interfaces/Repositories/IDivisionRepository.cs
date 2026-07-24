using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IDivisionRepository : IRepository<Division>
    {
        Task<List<Division>> GetAllOrderedAsync();
    }
}
