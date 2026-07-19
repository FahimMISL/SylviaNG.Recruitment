using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CandidateProfileService : ICandidateProfileService
    {
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly ITalentPoolCandidateRepository _talentPoolCandidateRepository;
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;

        public CandidateProfileService(
            ICandidateProfileRepository candidateProfileRepository,
            IJobApplicationRepository jobApplicationRepository,
            ITalentPoolCandidateRepository talentPoolCandidateRepository,
            ICurrentCandidateService currentCandidateService,
            IFileStorageService fileStorageService,
            IUnitOfWork unitOfWork)
        {
            _candidateProfileRepository = candidateProfileRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _talentPoolCandidateRepository = talentPoolCandidateRepository;
            _currentCandidateService = currentCandidateService;
            _fileStorageService = fileStorageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CandidateProfileResponse> GetMyProfileAsync()
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entity = await _candidateProfileRepository.GetByIdWithIncludeAsync(
                c => c.CandidateProfileId == profileId,
                c => c.Educations, c => c.WorkExperiences, c => c.Skills, c => c.Certifications, c => c.Documents)
                ?? throw new NotFoundException("CandidateProfile", profileId);

            var response = entity.ToResponse();
            response.HasSubmittedApplication = await HasSubmittedApplicationAsync(entity);
            return response;
        }

        public async Task<PagedResult<CandidateProfileSummaryResponse>> GetPagedAsync(PagedRequest request, List<long>? talentPoolIds = null)
        {
            var pagedResult = await _candidateProfileRepository.GetPagedAsync(request, talentPoolIds);

            return new PagedResult<CandidateProfileSummaryResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToSummaryResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<CandidateProfileDetailResponse> GetProfileDetailAsync(long candidateProfileId)
        {
            var entity = await _candidateProfileRepository.GetByIdWithIncludeAsync(
                c => c.CandidateProfileId == candidateProfileId,
                c => c.Educations, c => c.WorkExperiences, c => c.Skills, c => c.Certifications, c => c.Documents)
                ?? throw new NotFoundException("CandidateProfile", candidateProfileId);

            var applications = await _jobApplicationRepository.GetByCandidateEmailAsync(entity.Email);
            var poolMemberships = await _talentPoolCandidateRepository.GetAllByCandidateProfileIdAsync(candidateProfileId);

            return entity.ToDetailResponse(applications, poolMemberships);
        }

        public async Task UpdateHrNotesAsync(long candidateProfileId, string? hrNotes)
        {
            var entity = await _candidateProfileRepository.GetByIdAsync(candidateProfileId)
                ?? throw new NotFoundException("CandidateProfile", candidateProfileId);

            entity.HrNotes = hrNotes;
            entity.UpdatedAt = DateTime.UtcNow;

            _candidateProfileRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdatePersonalInfoAsync(CandidateProfilePersonalInfoUpdateRequest request)
        {
            var entity = await GetCurrentProfileEntityAsync();

            await EnsureFieldNotLockedIfChangedAsync(entity, "NationalId", entity.NationalId, request.NationalId);

            entity.ApplyPersonalInfoUpdate(request);
            entity.UpdatedAt = DateTime.UtcNow;

            _candidateProfileRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateContactAsync(CandidateProfileContactUpdateRequest request)
        {
            var entity = await GetCurrentProfileEntityAsync();

            await EnsureFieldNotLockedIfChangedAsync(entity, "Email", entity.Email, request.Email);
            await EnsureFieldNotLockedIfChangedAsync(entity, "Phone", entity.Phone, request.Phone, normalizeAsPhone: true);

            entity.ApplyContactUpdate(request);
            entity.UpdatedAt = DateTime.UtcNow;

            _candidateProfileRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<string> UploadPhotoAsync(IFormFile file)
        {
            return await UploadMediaAsync(file, "candidate-photos", (entity, path) => entity.ProfilePhotoPath = path, entity => entity.ProfilePhotoPath);
        }

        public async Task DeletePhotoAsync()
        {
            await DeleteMediaAsync(entity => entity.ProfilePhotoPath, (entity) => entity.ProfilePhotoPath = null);
        }

        public async Task<string> UploadSignatureAsync(IFormFile file)
        {
            return await UploadMediaAsync(file, "candidate-signatures", (entity, path) => entity.SignaturePath = path, entity => entity.SignaturePath);
        }

        public async Task DeleteSignatureAsync()
        {
            await DeleteMediaAsync(entity => entity.SignaturePath, (entity) => entity.SignaturePath = null);
        }

        private async Task<string> UploadMediaAsync(
            IFormFile file,
            string subFolderPrefix,
            Action<Domain.Entities.CandidateProfile, string> setPath,
            Func<Domain.Entities.CandidateProfile, string?> getPath)
        {
            var entity = await GetCurrentProfileEntityAsync();
            var oldFilePath = getPath(entity);

            await using var stream = file.OpenReadStream();
            var (_, filePath) = await _fileStorageService.SaveAsync(stream, file.FileName, $"{subFolderPrefix}/{entity.CandidateProfileId}");

            setPath(entity, filePath);
            entity.UpdatedAt = DateTime.UtcNow;

            _candidateProfileRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrEmpty(oldFilePath))
                await _fileStorageService.DeleteAsync(oldFilePath);

            return filePath;
        }

        private async Task DeleteMediaAsync(Func<Domain.Entities.CandidateProfile, string?> getPath, Action<Domain.Entities.CandidateProfile> clearPath)
        {
            var entity = await GetCurrentProfileEntityAsync();
            var filePath = getPath(entity);

            if (string.IsNullOrEmpty(filePath))
                return;

            clearPath(entity);
            entity.UpdatedAt = DateTime.UtcNow;

            _candidateProfileRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            await _fileStorageService.DeleteAsync(filePath);
        }

        private async Task<Domain.Entities.CandidateProfile> GetCurrentProfileEntityAsync()
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            return await _candidateProfileRepository.GetByIdAsync(profileId)
                ?? throw new NotFoundException("CandidateProfile", profileId);
        }

        // ── US-003: Identity-field lock (AC1/AC2/AC4) ──────────────────────
        // JobApplication has no FK to CandidateProfile - self-service lookups (GetMyApplicationsAsync,
        // WithdrawMyApplicationAsync) match by Email, so once a candidate has a submitted application,
        // Email/Phone/NationalId must not change or the candidate's own application history orphans
        // itself (it silently stops matching on the next lookup).

        private async Task EnsureFieldNotLockedIfChangedAsync(
            Domain.Entities.CandidateProfile entity,
            string fieldName,
            string? currentValue,
            string? newValue,
            bool normalizeAsPhone = false)
        {
            var changed = normalizeAsPhone
                ? !string.Equals(NormalizePhoneDigits(currentValue), NormalizePhoneDigits(newValue), StringComparison.Ordinal)
                : !string.Equals(currentValue, newValue, StringComparison.OrdinalIgnoreCase);

            if (!changed || !await HasSubmittedApplicationAsync(entity))
                return;

            throw new FluentValidation.ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure(
                    fieldName,
                    $"{fieldName} cannot be changed after you have submitted a job application. Contact HR support if this needs to be corrected.")
            });
        }

        private async Task<bool> HasSubmittedApplicationAsync(Domain.Entities.CandidateProfile entity)
        {
            var applications = await _jobApplicationRepository.GetByCandidateEmailAsync(entity.Email);
            return applications.Count > 0;
        }

        private static string? NormalizePhoneDigits(string? phone) =>
            phone == null ? null : new string(phone.Where(char.IsDigit).ToArray());
    }
}
