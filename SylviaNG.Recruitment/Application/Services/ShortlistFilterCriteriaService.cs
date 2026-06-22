using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ShortlistFilterCriteriaService : IShortlistFilterCriteriaService
    {
        private readonly IShortlistFilterCriteriaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ShortlistFilterCriteriaService(IShortlistFilterCriteriaRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ShortlistFilterCriteriaCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.ShortlistFilterCriteriaId;
        }

        public async Task UpdateAsync(long shortlistFilterCriteriaId, ShortlistFilterCriteriaUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(shortlistFilterCriteriaId)
                ?? throw new KeyNotFoundException($"ShortlistFilterCriteria with ID {shortlistFilterCriteriaId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long shortlistFilterCriteriaId)
        {
            var entity = await _repository.GetByIdAsync(shortlistFilterCriteriaId)
                ?? throw new KeyNotFoundException($"ShortlistFilterCriteria with ID {shortlistFilterCriteriaId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ShortlistFilterCriteriaResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<ShortlistFilterCriteriaResponse> GetByIdAsync(long shortlistFilterCriteriaId)
        {
            var entity = await _repository.GetByIdAsync(shortlistFilterCriteriaId)
                ?? throw new KeyNotFoundException($"ShortlistFilterCriteria with ID {shortlistFilterCriteriaId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<ShortlistFilterCriteriaResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<ShortlistFilterCriteriaResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
