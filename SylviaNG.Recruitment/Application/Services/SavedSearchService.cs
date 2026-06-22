using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class SavedSearchService : ISavedSearchService
    {
        private readonly ISavedSearchRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public SavedSearchService(ISavedSearchRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(SavedSearchCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.SavedSearchId;
        }

        public async Task UpdateAsync(long savedSearchId, SavedSearchUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(savedSearchId)
                ?? throw new KeyNotFoundException($"SavedSearch with ID {savedSearchId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long savedSearchId)
        {
            var entity = await _repository.GetByIdAsync(savedSearchId)
                ?? throw new KeyNotFoundException($"SavedSearch with ID {savedSearchId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<SavedSearchResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<SavedSearchResponse> GetByIdAsync(long savedSearchId)
        {
            var entity = await _repository.GetByIdAsync(savedSearchId)
                ?? throw new KeyNotFoundException($"SavedSearch with ID {savedSearchId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<SavedSearchResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<SavedSearchResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
