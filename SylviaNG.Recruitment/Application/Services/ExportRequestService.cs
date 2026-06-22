using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ExportRequestService : IExportRequestService
    {
        private readonly IExportRequestRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ExportRequestService(IExportRequestRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ExportRequestCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.ExportRequestId;
        }

        public async Task UpdateAsync(long exportRequestId, ExportRequestUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(exportRequestId)
                ?? throw new KeyNotFoundException($"ExportRequest with ID {exportRequestId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long exportRequestId)
        {
            var entity = await _repository.GetByIdAsync(exportRequestId)
                ?? throw new KeyNotFoundException($"ExportRequest with ID {exportRequestId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ExportRequestResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<ExportRequestResponse> GetByIdAsync(long exportRequestId)
        {
            var entity = await _repository.GetByIdAsync(exportRequestId)
                ?? throw new KeyNotFoundException($"ExportRequest with ID {exportRequestId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<ExportRequestResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<ExportRequestResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
