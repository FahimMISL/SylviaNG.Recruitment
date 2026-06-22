using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class DocumentTemplateService : IDocumentTemplateService
    {
        private readonly IDocumentTemplateRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DocumentTemplateService(IDocumentTemplateRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(DocumentTemplateCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.DocumentTemplateId;
        }

        public async Task UpdateAsync(long documentTemplateId, DocumentTemplateUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(documentTemplateId)
                ?? throw new KeyNotFoundException($"DocumentTemplate with ID {documentTemplateId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long documentTemplateId)
        {
            var entity = await _repository.GetByIdAsync(documentTemplateId)
                ?? throw new KeyNotFoundException($"DocumentTemplate with ID {documentTemplateId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<DocumentTemplateResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<DocumentTemplateResponse> GetByIdAsync(long documentTemplateId)
        {
            var entity = await _repository.GetByIdAsync(documentTemplateId)
                ?? throw new KeyNotFoundException($"DocumentTemplate with ID {documentTemplateId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<DocumentTemplateResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<DocumentTemplateResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
