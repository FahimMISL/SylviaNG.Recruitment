using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IThanaRepository : IRepository<Thana>
    {
        Task<List<Thana>> GetByDistrictIdOrderedAsync(long districtId);
    }
}
