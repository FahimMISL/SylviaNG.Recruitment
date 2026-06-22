using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CandidateDocumentService : ICandidateDocumentService
    {
        private readonly ICandidateDocumentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CandidateDocumentService(ICandidateDocumentRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(CandidateDocumentCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.CandidateDocumentId;
        }

        public async Task UpdateAsync(long candidateDocumentId, CandidateDocumentUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(candidateDocumentId)
                ?? throw new KeyNotFoundException($"CandidateDocument with ID {candidateDocumentId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long candidateDocumentId)
        {
            var entity = await _repository.GetByIdAsync(candidateDocumentId)
                ?? throw new KeyNotFoundException($"CandidateDocument with ID {candidateDocumentId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<CandidateDocumentResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<CandidateDocumentResponse> GetByIdAsync(long candidateDocumentId)
        {
            var entity = await _repository.GetByIdAsync(candidateDocumentId)
                ?? throw new KeyNotFoundException($"CandidateDocument with ID {candidateDocumentId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<CandidateDocumentResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<CandidateDocumentResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
