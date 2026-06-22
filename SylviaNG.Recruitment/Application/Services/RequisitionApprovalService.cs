using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class RequisitionApprovalService : IRequisitionApprovalService
    {
        private readonly IRequisitionApprovalRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RequisitionApprovalService(IRequisitionApprovalRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(RequisitionApprovalCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.RequisitionApprovalId;
        }

        public async Task UpdateAsync(long requisitionApprovalId, RequisitionApprovalUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(requisitionApprovalId)
                ?? throw new KeyNotFoundException($"RequisitionApproval with ID {requisitionApprovalId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long requisitionApprovalId)
        {
            var entity = await _repository.GetByIdAsync(requisitionApprovalId)
                ?? throw new KeyNotFoundException($"RequisitionApproval with ID {requisitionApprovalId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<RequisitionApprovalResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<RequisitionApprovalResponse> GetByIdAsync(long requisitionApprovalId)
        {
            var entity = await _repository.GetByIdAsync(requisitionApprovalId)
                ?? throw new KeyNotFoundException($"RequisitionApproval with ID {requisitionApprovalId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<RequisitionApprovalResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<RequisitionApprovalResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
