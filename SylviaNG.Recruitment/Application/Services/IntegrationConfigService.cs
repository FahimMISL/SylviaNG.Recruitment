using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class IntegrationConfigService : IIntegrationConfigService
    {
        private readonly IIntegrationConfigRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public IntegrationConfigService(IIntegrationConfigRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(IntegrationConfigCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.IntegrationConfigId;
        }

        public async Task UpdateAsync(long integrationConfigId, IntegrationConfigUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(integrationConfigId)
                ?? throw new KeyNotFoundException($"IntegrationConfig with ID {integrationConfigId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long integrationConfigId)
        {
            var entity = await _repository.GetByIdAsync(integrationConfigId)
                ?? throw new KeyNotFoundException($"IntegrationConfig with ID {integrationConfigId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<IntegrationConfigResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<IntegrationConfigResponse> GetByIdAsync(long integrationConfigId)
        {
            var entity = await _repository.GetByIdAsync(integrationConfigId)
                ?? throw new KeyNotFoundException($"IntegrationConfig with ID {integrationConfigId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<IntegrationConfigResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<IntegrationConfigResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
