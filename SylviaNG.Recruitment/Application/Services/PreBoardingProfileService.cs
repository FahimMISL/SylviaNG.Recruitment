using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class PreBoardingProfileService : IPreBoardingProfileService
    {
        private readonly IPreBoardingProfileRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public PreBoardingProfileService(IPreBoardingProfileRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(PreBoardingProfileCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.PreBoardingProfileId;
        }

        public async Task UpdateAsync(long preBoardingProfileId, PreBoardingProfileUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(preBoardingProfileId)
                ?? throw new KeyNotFoundException($"PreBoardingProfile with ID {preBoardingProfileId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long preBoardingProfileId)
        {
            var entity = await _repository.GetByIdAsync(preBoardingProfileId)
                ?? throw new KeyNotFoundException($"PreBoardingProfile with ID {preBoardingProfileId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<PreBoardingProfileResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<PreBoardingProfileResponse> GetByIdAsync(long preBoardingProfileId)
        {
            var entity = await _repository.GetByIdAsync(preBoardingProfileId)
                ?? throw new KeyNotFoundException($"PreBoardingProfile with ID {preBoardingProfileId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<PreBoardingProfileResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<PreBoardingProfileResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
