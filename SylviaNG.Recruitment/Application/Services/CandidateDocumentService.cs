using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Helpers;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CandidateDocumentService : ICandidateDocumentService
    {
        private readonly ICandidateDocumentRepository _candidateDocumentRepository;
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IPrivateFileAccessTokenService _privateFileAccessTokenService;
        private readonly PrivateFileAccessSettings _privateFileAccessSettings;
        private readonly IUnitOfWork _unitOfWork;

        public CandidateDocumentService(
            ICandidateDocumentRepository candidateDocumentRepository,
            ICurrentCandidateService currentCandidateService,
            IFileStorageService fileStorageService,
            IPrivateFileAccessTokenService privateFileAccessTokenService,
            IOptions<PrivateFileAccessSettings> privateFileAccessSettings,
            IUnitOfWork unitOfWork)
        {
            _candidateDocumentRepository = candidateDocumentRepository;
            _currentCandidateService = currentCandidateService;
            _fileStorageService = fileStorageService;
            _privateFileAccessTokenService = privateFileAccessTokenService;
            _privateFileAccessSettings = privateFileAccessSettings.Value;
            _unitOfWork = unitOfWork;
        }

        // FilesController requires a token/expires pair for any "candidate-documents/" key (see
        // its PrivateKeyPrefix check) - minted here, on the authenticated owner's own list/upload/
        // update call, rather than trusting the raw stored key alone as today's plain
        // ToResponse() does. The token is short-lived (PrivateFileAccess:ExpiryMinutes) so a
        // leaked/forwarded/referrer-logged URL stops working on its own.
        private CandidateDocumentResponse ToSecureResponse(CandidateDocument entity)
        {
            var response = entity.ToResponse();
            response.DownloadUrl = PrivateFileUrlSecurer.Secure(
                response.DownloadUrl, entity.FilePath, _privateFileAccessTokenService, _privateFileAccessSettings.ExpiryMinutes)
                ?? response.DownloadUrl;
            return response;
        }

        public async Task<List<CandidateDocumentResponse>> GetAllForCurrentCandidateAsync()
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entities = await _candidateDocumentRepository.GetAllByCandidateProfileIdAsync(profileId);
            return entities.Select(ToSecureResponse).ToList();
        }

        public async Task<CandidateDocumentResponse> UploadAsync(CandidateDocumentUploadRequest request)
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();

            await using var stream = request.File.OpenReadStream();
            var (storedFileName, filePath) = await _fileStorageService.SaveAsync(
                stream, request.File.FileName, $"candidate-documents/{profileId}");

            var entity = new CandidateDocument
            {
                CandidateProfileId = profileId,
                DocumentType = request.DocumentType,
                FileName = request.File.FileName,
                StoredFileName = storedFileName,
                FilePath = filePath,
                ContentType = request.File.ContentType,
                FileSizeBytes = request.File.Length,
                IsActive = true
            };

            await _candidateDocumentRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return ToSecureResponse(entity);
        }

        public async Task<CandidateDocumentResponse> UpdateAsync(long candidateDocumentId, CandidateDocumentUpdateRequest request)
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entity = await _candidateDocumentRepository.GetByIdAsync(candidateDocumentId);

            if (entity == null || entity.CandidateProfileId != profileId)
                throw new NotFoundException("CandidateDocument", candidateDocumentId);

            entity.DocumentType = request.DocumentType;
            entity.UpdatedAt = DateTime.UtcNow;

            string? oldFilePath = null;

            if (request.File != null)
            {
                oldFilePath = entity.FilePath;

                await using var stream = request.File.OpenReadStream();
                var (storedFileName, filePath) = await _fileStorageService.SaveAsync(
                    stream, request.File.FileName, $"candidate-documents/{profileId}");

                entity.FileName = request.File.FileName;
                entity.StoredFileName = storedFileName;
                entity.FilePath = filePath;
                entity.ContentType = request.File.ContentType;
                entity.FileSizeBytes = request.File.Length;
            }

            _candidateDocumentRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrEmpty(oldFilePath))
                await _fileStorageService.DeleteAsync(oldFilePath);

            return ToSecureResponse(entity);
        }

        public async Task DeleteAsync(long candidateDocumentId)
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entity = await _candidateDocumentRepository.GetByIdAsync(candidateDocumentId);

            if (entity == null || entity.CandidateProfileId != profileId)
                throw new NotFoundException("CandidateDocument", candidateDocumentId);

            _candidateDocumentRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();

            await _fileStorageService.DeleteAsync(entity.FilePath);
        }
    }
}
