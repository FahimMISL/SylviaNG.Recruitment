using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IDistrictRepository : IRepository<District>
    {
        Task<List<District>> GetByDivisionIdOrderedAsync(long divisionId);
    }
}
