using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CandidateExperienceService : ICandidateExperienceService
    {
        private readonly ICandidateExperienceRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CandidateExperienceService(ICandidateExperienceRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(CandidateExperienceCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.CandidateExperienceId;
        }

        public async Task UpdateAsync(long candidateExperienceId, CandidateExperienceUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(candidateExperienceId)
                ?? throw new KeyNotFoundException($"CandidateExperience with ID {candidateExperienceId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long candidateExperienceId)
        {
            var entity = await _repository.GetByIdAsync(candidateExperienceId)
                ?? throw new KeyNotFoundException($"CandidateExperience with ID {candidateExperienceId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<CandidateExperienceResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<CandidateExperienceResponse> GetByIdAsync(long candidateExperienceId)
        {
            var entity = await _repository.GetByIdAsync(candidateExperienceId)
                ?? throw new KeyNotFoundException($"CandidateExperience with ID {candidateExperienceId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<CandidateExperienceResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<CandidateExperienceResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
