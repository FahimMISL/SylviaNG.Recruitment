using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class DocumentAcceptanceService : IDocumentAcceptanceService
    {
        private readonly IDocumentAcceptanceRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DocumentAcceptanceService(IDocumentAcceptanceRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(DocumentAcceptanceCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.DocumentAcceptanceId;
        }

        public async Task UpdateAsync(long documentAcceptanceId, DocumentAcceptanceUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(documentAcceptanceId)
                ?? throw new KeyNotFoundException($"DocumentAcceptance with ID {documentAcceptanceId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long documentAcceptanceId)
        {
            var entity = await _repository.GetByIdAsync(documentAcceptanceId)
                ?? throw new KeyNotFoundException($"DocumentAcceptance with ID {documentAcceptanceId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<DocumentAcceptanceResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<DocumentAcceptanceResponse> GetByIdAsync(long documentAcceptanceId)
        {
            var entity = await _repository.GetByIdAsync(documentAcceptanceId)
                ?? throw new KeyNotFoundException($"DocumentAcceptance with ID {documentAcceptanceId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<DocumentAcceptanceResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<DocumentAcceptanceResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
