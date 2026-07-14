using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IApplicationStatusReasonRepository : IRepository<ApplicationStatusReason>
    {
        Task<List<ApplicationStatusReason>> GetActiveByStatusAsync(ApplicationStatusEnum status);
    }
}
