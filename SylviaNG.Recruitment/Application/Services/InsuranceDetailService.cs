using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class InsuranceDetailService : IInsuranceDetailService
    {
        private readonly IInsuranceDetailRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public InsuranceDetailService(IInsuranceDetailRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(InsuranceDetailCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.InsuranceDetailId;
        }

        public async Task UpdateAsync(long insuranceDetailId, InsuranceDetailUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(insuranceDetailId)
                ?? throw new KeyNotFoundException($"InsuranceDetail with ID {insuranceDetailId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long insuranceDetailId)
        {
            var entity = await _repository.GetByIdAsync(insuranceDetailId)
                ?? throw new KeyNotFoundException($"InsuranceDetail with ID {insuranceDetailId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<InsuranceDetailResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<InsuranceDetailResponse> GetByIdAsync(long insuranceDetailId)
        {
            var entity = await _repository.GetByIdAsync(insuranceDetailId)
                ?? throw new KeyNotFoundException($"InsuranceDetail with ID {insuranceDetailId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<InsuranceDetailResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<InsuranceDetailResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
