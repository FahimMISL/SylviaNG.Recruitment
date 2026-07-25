using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class DocumentTemplateService : IDocumentTemplateService
    {
        private readonly IDocumentTemplateRepository _documentTemplateRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DocumentTemplateService(IDocumentTemplateRepository documentTemplateRepository, IUnitOfWork unitOfWork)
        {
            _documentTemplateRepository = documentTemplateRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(DocumentTemplateCreateRequest request)
        {
            var exists = await _documentTemplateRepository.ExistsByCodeAsync(request.Code);
            if (exists)
                throw new DuplicateException("DocumentTemplate", "Code", request.Code);

            var entity = request.ToEntity();
            await _documentTemplateRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            await _documentTemplateRepository.AddVersionAsync(new DocumentTemplateVersion
            {
                DocumentTemplateId = entity.DocumentTemplateId,
                VersionNumber = 1,
                Body = entity.Body,
            });
            await _unitOfWork.SaveChangesAsync();

            return entity.DocumentTemplateId;
        }

        public async Task UpdateAsync(long documentTemplateId, DocumentTemplateUpdateRequest request)
        {
            var entity = await _documentTemplateRepository.GetByIdAsync(documentTemplateId)
                ?? throw new NotFoundException("DocumentTemplate", documentTemplateId);

            entity.Name = request.Name;
            entity.Body = request.Body;
            entity.IsActive = request.IsActive;
            entity.CurrentVersionNumber += 1;
            _documentTemplateRepository.Update(entity);

            await _documentTemplateRepository.AddVersionAsync(new DocumentTemplateVersion
            {
                DocumentTemplateId = entity.DocumentTemplateId,
                VersionNumber = entity.CurrentVersionNumber,
                Body = entity.Body,
            });

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long documentTemplateId)
        {
            var entity = await _documentTemplateRepository.GetByIdAsync(documentTemplateId)
                ?? throw new NotFoundException("DocumentTemplate", documentTemplateId);

            var usageCount = await _documentTemplateRepository.CountOfferLetterUsageAsync(documentTemplateId);
            if (usageCount > 0)
                throw new ResourceInUseException("DocumentTemplate", documentTemplateId, usageCount);

            _documentTemplateRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<DocumentTemplateResponse>> GetAllAsync()
        {
            var entities = await _documentTemplateRepository.GetAllOrderedAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<DocumentTemplateResponse> GetByIdAsync(long documentTemplateId)
        {
            var entity = await _documentTemplateRepository.GetByIdAsync(documentTemplateId)
                ?? throw new NotFoundException("DocumentTemplate", documentTemplateId);

            return entity.ToResponse();
        }

        public async Task<List<DocumentTemplateVersionResponse>> GetVersionsAsync(long documentTemplateId)
        {
            var exists = await _documentTemplateRepository.GetByIdAsync(documentTemplateId)
                ?? throw new NotFoundException("DocumentTemplate", documentTemplateId);

            var versions = await _documentTemplateRepository.GetVersionsOrderedAsync(documentTemplateId);
            return versions.Select(v => v.ToResponse()).ToList();
        }
    }
}
