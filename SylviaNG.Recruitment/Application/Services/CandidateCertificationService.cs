using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CandidateCertificationService : ICandidateCertificationService
    {
        private readonly ICandidateCertificationRepository _candidateCertificationRepository;
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;

        public CandidateCertificationService(
            ICandidateCertificationRepository candidateCertificationRepository,
            ICurrentCandidateService currentCandidateService,
            IFileStorageService fileStorageService,
            IUnitOfWork unitOfWork)
        {
            _candidateCertificationRepository = candidateCertificationRepository;
            _currentCandidateService = currentCandidateService;
            _fileStorageService = fileStorageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CandidateCertificationResponse>> GetAllForCurrentCandidateAsync()
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entities = await _candidateCertificationRepository.GetAllByCandidateProfileIdAsync(profileId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<long> CreateAsync(CandidateCertificationCreateRequest request)
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entity = request.ToEntity(profileId);

            if (request.CertificateFile != null)
            {
                await using var stream = request.CertificateFile.OpenReadStream();
                var (_, filePath) = await _fileStorageService.SaveAsync(
                    stream, request.CertificateFile.FileName, $"candidate-certifications/{profileId}");
                entity.CertificateFilePath = filePath;
            }

            await _candidateCertificationRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.CandidateCertificationId;
        }

        public async Task UpdateAsync(long candidateCertificationId, CandidateCertificationUpdateRequest request)
        {
            var (entity, profileId) = await GetOwnedEntityAsync(candidateCertificationId);

            entity.ApplyUpdate(request);
            entity.UpdatedAt = DateTime.UtcNow;

            if (request.CertificateFile != null)
            {
                var oldFilePath = entity.CertificateFilePath;

                await using var stream = request.CertificateFile.OpenReadStream();
                var (_, filePath) = await _fileStorageService.SaveAsync(
                    stream, request.CertificateFile.FileName, $"candidate-certifications/{profileId}");
                entity.CertificateFilePath = filePath;

                if (!string.IsNullOrEmpty(oldFilePath))
                    await _fileStorageService.DeleteAsync(oldFilePath);
            }

            _candidateCertificationRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long candidateCertificationId)
        {
            var (entity, _) = await GetOwnedEntityAsync(candidateCertificationId);

            _candidateCertificationRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrEmpty(entity.CertificateFilePath))
                await _fileStorageService.DeleteAsync(entity.CertificateFilePath);
        }

        private async Task<(CandidateCertification Entity, long ProfileId)> GetOwnedEntityAsync(long candidateCertificationId)
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entity = await _candidateCertificationRepository.GetByIdAsync(candidateCertificationId);

            if (entity == null || entity.CandidateProfileId != profileId)
                throw new NotFoundException("CandidateCertification", candidateCertificationId);

            return (entity, profileId);
        }
    }
}
