using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ImpersonationLogService : IImpersonationLogService
    {
        private readonly IImpersonationLogRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ImpersonationLogService(IImpersonationLogRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ImpersonationLogCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.ImpersonationLogId;
        }

        public async Task UpdateAsync(long impersonationLogId, ImpersonationLogUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(impersonationLogId)
                ?? throw new KeyNotFoundException($"ImpersonationLog with ID {impersonationLogId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long impersonationLogId)
        {
            var entity = await _repository.GetByIdAsync(impersonationLogId)
                ?? throw new KeyNotFoundException($"ImpersonationLog with ID {impersonationLogId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ImpersonationLogResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<ImpersonationLogResponse> GetByIdAsync(long impersonationLogId)
        {
            var entity = await _repository.GetByIdAsync(impersonationLogId)
                ?? throw new KeyNotFoundException($"ImpersonationLog with ID {impersonationLogId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<ImpersonationLogResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<ImpersonationLogResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
