using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class GeneratedDocumentService : IGeneratedDocumentService
    {
        private readonly IGeneratedDocumentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public GeneratedDocumentService(IGeneratedDocumentRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(GeneratedDocumentCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.GeneratedDocumentId;
        }

        public async Task UpdateAsync(long generatedDocumentId, GeneratedDocumentUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(generatedDocumentId)
                ?? throw new KeyNotFoundException($"GeneratedDocument with ID {generatedDocumentId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long generatedDocumentId)
        {
            var entity = await _repository.GetByIdAsync(generatedDocumentId)
                ?? throw new KeyNotFoundException($"GeneratedDocument with ID {generatedDocumentId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<GeneratedDocumentResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<GeneratedDocumentResponse> GetByIdAsync(long generatedDocumentId)
        {
            var entity = await _repository.GetByIdAsync(generatedDocumentId)
                ?? throw new KeyNotFoundException($"GeneratedDocument with ID {generatedDocumentId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<GeneratedDocumentResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<GeneratedDocumentResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
