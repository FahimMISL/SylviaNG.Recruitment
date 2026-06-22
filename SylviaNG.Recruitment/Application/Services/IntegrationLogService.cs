using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class IntegrationLogService : IIntegrationLogService
    {
        private readonly IIntegrationLogRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public IntegrationLogService(IIntegrationLogRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(IntegrationLogCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.IntegrationLogId;
        }

        public async Task UpdateAsync(long integrationLogId, IntegrationLogUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(integrationLogId)
                ?? throw new KeyNotFoundException($"IntegrationLog with ID {integrationLogId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long integrationLogId)
        {
            var entity = await _repository.GetByIdAsync(integrationLogId)
                ?? throw new KeyNotFoundException($"IntegrationLog with ID {integrationLogId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<IntegrationLogResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<IntegrationLogResponse> GetByIdAsync(long integrationLogId)
        {
            var entity = await _repository.GetByIdAsync(integrationLogId)
                ?? throw new KeyNotFoundException($"IntegrationLog with ID {integrationLogId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<IntegrationLogResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<IntegrationLogResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
