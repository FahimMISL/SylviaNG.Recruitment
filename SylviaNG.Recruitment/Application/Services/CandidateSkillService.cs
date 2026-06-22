using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateSkills.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CandidateSkillService : ICandidateSkillService
    {
        private readonly ICandidateSkillRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CandidateSkillService(ICandidateSkillRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(CandidateSkillCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.CandidateSkillId;
        }

        public async Task UpdateAsync(long candidateSkillId, CandidateSkillUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(candidateSkillId)
                ?? throw new KeyNotFoundException($"CandidateSkill with ID {candidateSkillId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long candidateSkillId)
        {
            var entity = await _repository.GetByIdAsync(candidateSkillId)
                ?? throw new KeyNotFoundException($"CandidateSkill with ID {candidateSkillId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<CandidateSkillResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<CandidateSkillResponse> GetByIdAsync(long candidateSkillId)
        {
            var entity = await _repository.GetByIdAsync(candidateSkillId)
                ?? throw new KeyNotFoundException($"CandidateSkill with ID {candidateSkillId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<CandidateSkillResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<CandidateSkillResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
