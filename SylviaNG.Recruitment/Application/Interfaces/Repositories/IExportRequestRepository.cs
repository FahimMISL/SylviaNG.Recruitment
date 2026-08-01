using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IExportRequestRepository : IRepository<ExportRequest>
    {
        /// <summary>Shared Admin/HR view, newest first - same shared-inbox convention as
        /// NotificationLog (not scoped per-requester).</summary>
        Task<PagedResult<ExportRequest>> GetPagedAsync(int page, int pageSize, ExportRequestStatusEnum? status);

        /// <summary>US-104: oldest-first so the queue drains in request order.</summary>
        Task<List<ExportRequest>> GetPendingOldestFirstAsync(int take);

        /// <summary>US-104 retention sweep: Completed/Failed rows past their ExpiresAt.</summary>
        Task<List<ExportRequest>> GetExpiredAsync(DateTime nowUtc);
    }
}
