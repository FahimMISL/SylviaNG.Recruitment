using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IStaffProfileRepository : IRepository<StaffProfile>
    {
        Task<StaffProfile?> GetByKeycloakSubjectIdAsync(string keycloakSubjectId);
    }
}
