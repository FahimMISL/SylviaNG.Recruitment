using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateEducations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CandidateEducationService : ICandidateEducationService
    {
        private readonly ICandidateEducationRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CandidateEducationService(ICandidateEducationRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(CandidateEducationCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.CandidateEducationId;
        }

        public async Task UpdateAsync(long candidateEducationId, CandidateEducationUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(candidateEducationId)
                ?? throw new KeyNotFoundException($"CandidateEducation with ID {candidateEducationId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long candidateEducationId)
        {
            var entity = await _repository.GetByIdAsync(candidateEducationId)
                ?? throw new KeyNotFoundException($"CandidateEducation with ID {candidateEducationId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<CandidateEducationResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<CandidateEducationResponse> GetByIdAsync(long candidateEducationId)
        {
            var entity = await _repository.GetByIdAsync(candidateEducationId)
                ?? throw new KeyNotFoundException($"CandidateEducation with ID {candidateEducationId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<CandidateEducationResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<CandidateEducationResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
