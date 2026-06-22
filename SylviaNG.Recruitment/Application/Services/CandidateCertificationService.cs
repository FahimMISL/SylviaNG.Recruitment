using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CandidateCertificationService : ICandidateCertificationService
    {
        private readonly ICandidateCertificationRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CandidateCertificationService(ICandidateCertificationRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(CandidateCertificationCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.CandidateCertificationId;
        }

        public async Task UpdateAsync(long candidateCertificationId, CandidateCertificationUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(candidateCertificationId)
                ?? throw new KeyNotFoundException($"CandidateCertification with ID {candidateCertificationId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long candidateCertificationId)
        {
            var entity = await _repository.GetByIdAsync(candidateCertificationId)
                ?? throw new KeyNotFoundException($"CandidateCertification with ID {candidateCertificationId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<CandidateCertificationResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<CandidateCertificationResponse> GetByIdAsync(long candidateCertificationId)
        {
            var entity = await _repository.GetByIdAsync(candidateCertificationId)
                ?? throw new KeyNotFoundException($"CandidateCertification with ID {candidateCertificationId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<CandidateCertificationResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<CandidateCertificationResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
