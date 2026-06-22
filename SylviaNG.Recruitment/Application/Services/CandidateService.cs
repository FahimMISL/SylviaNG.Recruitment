using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.Candidates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CandidateService(ICandidateRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(CandidateCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.CandidateId;
        }

        public async Task UpdateAsync(long candidateId, CandidateUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(candidateId)
                ?? throw new KeyNotFoundException($"Candidate with ID {candidateId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long candidateId)
        {
            var entity = await _repository.GetByIdAsync(candidateId)
                ?? throw new KeyNotFoundException($"Candidate with ID {candidateId} not found.");
            entity.IsActive = false;
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<CandidateResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Where(e => e.IsActive).Select(e => e.ToResponse()).ToList();
        }

        public async Task<CandidateResponse> GetByIdAsync(long candidateId)
        {
            var entity = await _repository.GetByIdAsync(candidateId)
                ?? throw new KeyNotFoundException($"Candidate with ID {candidateId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<CandidateResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().Where(e => e.IsActive).ToPaginatedResultAsync(request);
            return new PagedResult<CandidateResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<long> AutoProvisionAsync(CandidateAutoProvisionRequest request)
        {
            // Check if candidate with this email already exists
            var existing = await _repository.GetByEmailAsync(request.Email);
            if (existing != null)
            {
                // If candidate exists but has no KeycloakUserId, link it
                if (string.IsNullOrEmpty(existing.KeycloakUserId))
                {
                    existing.KeycloakUserId = request.KeycloakUserId;
                    _repository.Update(existing);
                    await _unitOfWork.SaveChangesAsync();
                }
                return existing.CandidateId;
            }

            // Create new candidate with minimal profile
            var entity = new Domain.Entities.Candidate
            {
                KeycloakUserId = request.KeycloakUserId,
                FullName = request.FullName,
                Email = request.Email,
                CandidateType = Domain.Enums.CandidateTypeEnum.External,
                IsActive = true,
                ProfileCompletenessPercent = 0
            };

            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.CandidateId;
        }
    }
}
