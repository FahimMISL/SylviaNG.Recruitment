using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class ExportRequestRepository : Repository<ExportRequest>, IExportRequestRepository
    {
        public ExportRequestRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<ExportRequest>> GetPagedAsync(int page, int pageSize, ExportRequestStatusEnum? status)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (status.HasValue)
                query = query.Where(e => e.Status == status.Value);

            return await query
                .OrderByDescending(e => e.RequestedAt)
                .ToPaginatedResultAsync(new PagedRequest { Page = page, PageSize = pageSize });
        }

        public async Task<List<ExportRequest>> GetPendingOldestFirstAsync(int take)
        {
            return await _dbSet
                .Where(e => e.Status == ExportRequestStatusEnum.Pending)
                .OrderBy(e => e.RequestedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<ExportRequest>> GetExpiredAsync(DateTime nowUtc)
        {
            return await _dbSet
                .Where(e =>
                    (e.Status == ExportRequestStatusEnum.Completed || e.Status == ExportRequestStatusEnum.Failed)
                    && e.ExpiresAt <= nowUtc)
                .ToListAsync();
        }
    }
}
