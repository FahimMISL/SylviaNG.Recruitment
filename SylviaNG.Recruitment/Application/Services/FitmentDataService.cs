using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class FitmentDataService : IFitmentDataService
    {
        private readonly IFitmentDataRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public FitmentDataService(IFitmentDataRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(FitmentDataCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.FitmentDataId;
        }

        public async Task UpdateAsync(long fitmentDataId, FitmentDataUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(fitmentDataId)
                ?? throw new KeyNotFoundException($"FitmentData with ID {fitmentDataId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long fitmentDataId)
        {
            var entity = await _repository.GetByIdAsync(fitmentDataId)
                ?? throw new KeyNotFoundException($"FitmentData with ID {fitmentDataId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<FitmentDataResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<FitmentDataResponse> GetByIdAsync(long fitmentDataId)
        {
            var entity = await _repository.GetByIdAsync(fitmentDataId)
                ?? throw new KeyNotFoundException($"FitmentData with ID {fitmentDataId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<FitmentDataResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<FitmentDataResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
