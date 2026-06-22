using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class TalentPoolCandidateService : ITalentPoolCandidateService
    {
        private readonly ITalentPoolCandidateRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public TalentPoolCandidateService(ITalentPoolCandidateRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(TalentPoolCandidateCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.TalentPoolCandidateId;
        }

        public async Task UpdateAsync(long talentPoolCandidateId, TalentPoolCandidateUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(talentPoolCandidateId)
                ?? throw new KeyNotFoundException($"TalentPoolCandidate with ID {talentPoolCandidateId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long talentPoolCandidateId)
        {
            var entity = await _repository.GetByIdAsync(talentPoolCandidateId)
                ?? throw new KeyNotFoundException($"TalentPoolCandidate with ID {talentPoolCandidateId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<TalentPoolCandidateResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<TalentPoolCandidateResponse> GetByIdAsync(long talentPoolCandidateId)
        {
            var entity = await _repository.GetByIdAsync(talentPoolCandidateId)
                ?? throw new KeyNotFoundException($"TalentPoolCandidate with ID {talentPoolCandidateId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<TalentPoolCandidateResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<TalentPoolCandidateResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
