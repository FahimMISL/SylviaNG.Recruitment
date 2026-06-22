using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class OnboardingRecordService : IOnboardingRecordService
    {
        private readonly IOnboardingRecordRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public OnboardingRecordService(IOnboardingRecordRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(OnboardingRecordCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.OnboardingRecordId;
        }

        public async Task UpdateAsync(long onboardingRecordId, OnboardingRecordUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(onboardingRecordId)
                ?? throw new KeyNotFoundException($"OnboardingRecord with ID {onboardingRecordId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long onboardingRecordId)
        {
            var entity = await _repository.GetByIdAsync(onboardingRecordId)
                ?? throw new KeyNotFoundException($"OnboardingRecord with ID {onboardingRecordId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<OnboardingRecordResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<OnboardingRecordResponse> GetByIdAsync(long onboardingRecordId)
        {
            var entity = await _repository.GetByIdAsync(onboardingRecordId)
                ?? throw new KeyNotFoundException($"OnboardingRecord with ID {onboardingRecordId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<OnboardingRecordResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<OnboardingRecordResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
