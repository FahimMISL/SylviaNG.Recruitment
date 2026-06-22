using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ShortlistFilterService : IShortlistFilterService
    {
        private readonly IShortlistFilterRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ShortlistFilterService(IShortlistFilterRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ShortlistFilterCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.ShortlistFilterId;
        }

        public async Task UpdateAsync(long shortlistFilterId, ShortlistFilterUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(shortlistFilterId)
                ?? throw new KeyNotFoundException($"ShortlistFilter with ID {shortlistFilterId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long shortlistFilterId)
        {
            var entity = await _repository.GetByIdAsync(shortlistFilterId)
                ?? throw new KeyNotFoundException($"ShortlistFilter with ID {shortlistFilterId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ShortlistFilterResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<ShortlistFilterResponse> GetByIdAsync(long shortlistFilterId)
        {
            var entity = await _repository.GetByIdAsync(shortlistFilterId)
                ?? throw new KeyNotFoundException($"ShortlistFilter with ID {shortlistFilterId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<ShortlistFilterResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<ShortlistFilterResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
