using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.Requisitions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class RequisitionService : IRequisitionService
    {
        private readonly IRequisitionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RequisitionService(IRequisitionRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(RequisitionCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.RequisitionId;
        }

        public async Task UpdateAsync(long requisitionId, RequisitionUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(requisitionId)
                ?? throw new KeyNotFoundException($"Requisition with ID {requisitionId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long requisitionId)
        {
            var entity = await _repository.GetByIdAsync(requisitionId)
                ?? throw new KeyNotFoundException($"Requisition with ID {requisitionId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<RequisitionResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<RequisitionResponse> GetByIdAsync(long requisitionId)
        {
            var entity = await _repository.GetByIdAsync(requisitionId)
                ?? throw new KeyNotFoundException($"Requisition with ID {requisitionId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<RequisitionResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<RequisitionResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
