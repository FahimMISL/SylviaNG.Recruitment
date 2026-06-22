using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class TalentPoolService : ITalentPoolService
    {
        private readonly ITalentPoolRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public TalentPoolService(ITalentPoolRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(TalentPoolCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.TalentPoolId;
        }

        public async Task UpdateAsync(long talentPoolId, TalentPoolUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(talentPoolId)
                ?? throw new KeyNotFoundException($"TalentPool with ID {talentPoolId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long talentPoolId)
        {
            var entity = await _repository.GetByIdAsync(talentPoolId)
                ?? throw new KeyNotFoundException($"TalentPool with ID {talentPoolId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<TalentPoolResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<TalentPoolResponse> GetByIdAsync(long talentPoolId)
        {
            var entity = await _repository.GetByIdAsync(talentPoolId)
                ?? throw new KeyNotFoundException($"TalentPool with ID {talentPoolId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<TalentPoolResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<TalentPoolResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
