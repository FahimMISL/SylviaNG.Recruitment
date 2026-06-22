using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class DashboardWidgetService : IDashboardWidgetService
    {
        private readonly IDashboardWidgetRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DashboardWidgetService(IDashboardWidgetRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(DashboardWidgetCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.DashboardWidgetId;
        }

        public async Task UpdateAsync(long dashboardWidgetId, DashboardWidgetUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(dashboardWidgetId)
                ?? throw new KeyNotFoundException($"DashboardWidget with ID {dashboardWidgetId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long dashboardWidgetId)
        {
            var entity = await _repository.GetByIdAsync(dashboardWidgetId)
                ?? throw new KeyNotFoundException($"DashboardWidget with ID {dashboardWidgetId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<DashboardWidgetResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<DashboardWidgetResponse> GetByIdAsync(long dashboardWidgetId)
        {
            var entity = await _repository.GetByIdAsync(dashboardWidgetId)
                ?? throw new KeyNotFoundException($"DashboardWidget with ID {dashboardWidgetId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<DashboardWidgetResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<DashboardWidgetResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
