using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class RequisitionAttachmentService : IRequisitionAttachmentService
    {
        private readonly IRequisitionAttachmentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RequisitionAttachmentService(IRequisitionAttachmentRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(RequisitionAttachmentCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.RequisitionAttachmentId;
        }

        public async Task UpdateAsync(long requisitionAttachmentId, RequisitionAttachmentUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(requisitionAttachmentId)
                ?? throw new KeyNotFoundException($"RequisitionAttachment with ID {requisitionAttachmentId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long requisitionAttachmentId)
        {
            var entity = await _repository.GetByIdAsync(requisitionAttachmentId)
                ?? throw new KeyNotFoundException($"RequisitionAttachment with ID {requisitionAttachmentId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<RequisitionAttachmentResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<RequisitionAttachmentResponse> GetByIdAsync(long requisitionAttachmentId)
        {
            var entity = await _repository.GetByIdAsync(requisitionAttachmentId)
                ?? throw new KeyNotFoundException($"RequisitionAttachment with ID {requisitionAttachmentId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<RequisitionAttachmentResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<RequisitionAttachmentResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
