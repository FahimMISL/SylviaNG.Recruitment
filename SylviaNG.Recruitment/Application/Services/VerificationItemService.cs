using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.VerificationItems.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class VerificationItemService : IVerificationItemService
    {
        private readonly IVerificationItemRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public VerificationItemService(IVerificationItemRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(VerificationItemCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.VerificationItemId;
        }

        public async Task UpdateAsync(long verificationItemId, VerificationItemUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(verificationItemId)
                ?? throw new KeyNotFoundException($"VerificationItem with ID {verificationItemId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long verificationItemId)
        {
            var entity = await _repository.GetByIdAsync(verificationItemId)
                ?? throw new KeyNotFoundException($"VerificationItem with ID {verificationItemId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<VerificationItemResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<VerificationItemResponse> GetByIdAsync(long verificationItemId)
        {
            var entity = await _repository.GetByIdAsync(verificationItemId)
                ?? throw new KeyNotFoundException($"VerificationItem with ID {verificationItemId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<VerificationItemResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<VerificationItemResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
