using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class RequisitionStageConfigService : IRequisitionStageConfigService
    {
        private readonly IRequisitionStageConfigRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RequisitionStageConfigService(IRequisitionStageConfigRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(RequisitionStageConfigCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.RequisitionStageConfigId;
        }

        public async Task UpdateAsync(long requisitionStageConfigId, RequisitionStageConfigUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(requisitionStageConfigId)
                ?? throw new KeyNotFoundException($"RequisitionStageConfig with ID {requisitionStageConfigId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long requisitionStageConfigId)
        {
            var entity = await _repository.GetByIdAsync(requisitionStageConfigId)
                ?? throw new KeyNotFoundException($"RequisitionStageConfig with ID {requisitionStageConfigId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<RequisitionStageConfigResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<RequisitionStageConfigResponse> GetByIdAsync(long requisitionStageConfigId)
        {
            var entity = await _repository.GetByIdAsync(requisitionStageConfigId)
                ?? throw new KeyNotFoundException($"RequisitionStageConfig with ID {requisitionStageConfigId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<RequisitionStageConfigResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<RequisitionStageConfigResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
