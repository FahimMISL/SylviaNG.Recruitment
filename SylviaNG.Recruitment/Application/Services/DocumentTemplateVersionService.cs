using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class DocumentTemplateVersionService : IDocumentTemplateVersionService
    {
        private readonly IDocumentTemplateVersionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DocumentTemplateVersionService(IDocumentTemplateVersionRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(DocumentTemplateVersionCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.DocumentTemplateVersionId;
        }

        public async Task UpdateAsync(long documentTemplateVersionId, DocumentTemplateVersionUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(documentTemplateVersionId)
                ?? throw new KeyNotFoundException($"DocumentTemplateVersion with ID {documentTemplateVersionId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long documentTemplateVersionId)
        {
            var entity = await _repository.GetByIdAsync(documentTemplateVersionId)
                ?? throw new KeyNotFoundException($"DocumentTemplateVersion with ID {documentTemplateVersionId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<DocumentTemplateVersionResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<DocumentTemplateVersionResponse> GetByIdAsync(long documentTemplateVersionId)
        {
            var entity = await _repository.GetByIdAsync(documentTemplateVersionId)
                ?? throw new KeyNotFoundException($"DocumentTemplateVersion with ID {documentTemplateVersionId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<DocumentTemplateVersionResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<DocumentTemplateVersionResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
