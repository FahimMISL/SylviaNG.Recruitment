using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class FinalSelectionPoolService : IFinalSelectionPoolService
    {
        private readonly IFinalSelectionPoolRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public FinalSelectionPoolService(IFinalSelectionPoolRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(FinalSelectionPoolCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.FinalSelectionPoolId;
        }

        public async Task UpdateAsync(long finalSelectionPoolId, FinalSelectionPoolUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(finalSelectionPoolId)
                ?? throw new KeyNotFoundException($"FinalSelectionPool with ID {finalSelectionPoolId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long finalSelectionPoolId)
        {
            var entity = await _repository.GetByIdAsync(finalSelectionPoolId)
                ?? throw new KeyNotFoundException($"FinalSelectionPool with ID {finalSelectionPoolId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<FinalSelectionPoolResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<FinalSelectionPoolResponse> GetByIdAsync(long finalSelectionPoolId)
        {
            var entity = await _repository.GetByIdAsync(finalSelectionPoolId)
                ?? throw new KeyNotFoundException($"FinalSelectionPool with ID {finalSelectionPoolId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<FinalSelectionPoolResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<FinalSelectionPoolResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
