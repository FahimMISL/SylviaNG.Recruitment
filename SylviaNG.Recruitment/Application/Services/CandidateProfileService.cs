using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Externals;
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
        private readonly IAutoShortlistRunRepository _autoShortlistRunRepository;
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICoreGrpcClient _coreGrpcClient;
        private readonly ICandidateProfilePdfGeneratorService _candidateProfilePdfGeneratorService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CandidateProfileService> _logger;

        public CandidateProfileService(
            ICandidateProfileRepository candidateProfileRepository,
            IJobApplicationRepository jobApplicationRepository,
            ITalentPoolCandidateRepository talentPoolCandidateRepository,
            IAutoShortlistRunRepository autoShortlistRunRepository,
            ICurrentCandidateService currentCandidateService,
            IFileStorageService fileStorageService,
            ICoreGrpcClient coreGrpcClient,
            ICandidateProfilePdfGeneratorService candidateProfilePdfGeneratorService,
            IUnitOfWork unitOfWork,
            ILogger<CandidateProfileService> logger)
        {
            _candidateProfileRepository = candidateProfileRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _talentPoolCandidateRepository = talentPoolCandidateRepository;
            _autoShortlistRunRepository = autoShortlistRunRepository;
            _currentCandidateService = currentCandidateService;
            _fileStorageService = fileStorageService;
            _coreGrpcClient = coreGrpcClient;
            _candidateProfilePdfGeneratorService = candidateProfilePdfGeneratorService;
            _unitOfWork = unitOfWork;
            _logger = logger;
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
            (response.DepartmentName, response.DesignationName) = await ResolveOrgNamesAsync(entity);
            return response;
        }

        public async Task<PagedResult<CandidateProfileSummaryResponse>> GetPagedAsync(PagedRequest request, List<long>? talentPoolIds = null, List<string>? tags = null)
        {
            var pagedResult = await _candidateProfileRepository.GetPagedAsync(request, talentPoolIds, tags);

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
                c => c.Educations, c => c.WorkExperiences, c => c.Skills, c => c.Certifications, c => c.Documents, c => c.Tags)
                ?? throw new NotFoundException("CandidateProfile", candidateProfileId);

            var applications = await _jobApplicationRepository.GetByCandidateAsync(candidateProfileId, entity.Email);
            var poolMemberships = await _talentPoolCandidateRepository.GetAllByCandidateProfileIdAsync(candidateProfileId);

            var response = entity.ToDetailResponse(applications, poolMemberships);
            (response.DepartmentName, response.DesignationName) = await ResolveOrgNamesAsync(entity);
            return response;
        }

        public async Task<CandidateProfileDownloadResponse> DownloadProfilePdfAsync(long candidateProfileId)
        {
            var profiles = await _candidateProfileRepository.GetByIdsWithDetailsAsync(new[] { candidateProfileId });
            var entity = profiles.FirstOrDefault()
                ?? throw new NotFoundException("CandidateProfile", candidateProfileId);

            var screeningScore = await ResolveLatestScreeningScoreAsync(candidateProfileId, entity.Email);
            var content = _candidateProfilePdfGeneratorService.Generate(entity, screeningScore);
            var safeName = System.Text.RegularExpressions.Regex.Replace(entity.FullName, @"[^a-zA-Z0-9\-]+", "_").Trim('_');
            if (string.IsNullOrEmpty(safeName))
                safeName = "candidate";

            return new CandidateProfileDownloadResponse
            {
                Content = content,
                ContentType = "application/pdf",
                FileName = $"{safeName}_{candidateProfileId}_Profile.pdf"
            };
        }

        // US-103 AC2: no direct score field on CandidateProfile - AutoShortlistResult is keyed by
        // JobApplicationId/JobPostingId, so this walks the candidate's most-recently-applied
        // applications and returns the first scored one found (most recent application takes
        // priority over an older one that happens to also be scored).
        private async Task<int?> ResolveLatestScreeningScoreAsync(long candidateProfileId, string email)
        {
            var applications = await _jobApplicationRepository.GetByCandidateAsync(candidateProfileId, email);
            var byRecency = applications.OrderByDescending(a => a.AppliedDate).ToList();

            var scoresByJobPosting = new Dictionary<long, Dictionary<long, int>>();

            foreach (var application in byRecency)
            {
                if (!scoresByJobPosting.TryGetValue(application.JobPostingId, out var scores))
                {
                    scores = await _autoShortlistRunRepository.GetLatestScoresByJobPostingIdAsync(application.JobPostingId);
                    scoresByJobPosting[application.JobPostingId] = scores;
                }

                if (scores.TryGetValue(application.JobApplicationId, out var score))
                    return score;
            }

            return null;
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

        public async Task MarkInternalAsync(long candidateProfileId)
        {
            var entity = await _candidateProfileRepository.GetByIdAsync(candidateProfileId)
                ?? throw new NotFoundException("CandidateProfile", candidateProfileId);

            entity.IsManuallyInternal = true;
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

        // US-005 AC1: resolve Department/Designation display names via Core gRPC. Best-effort -
        // if the Core service is unreachable, the profile still loads (names just stay null),
        // same "best-effort side-effect" tolerance as resume-text extraction elsewhere.
        private async Task<(string? DepartmentName, string? DesignationName)> ResolveOrgNamesAsync(Domain.Entities.CandidateProfile entity)
        {
            if (entity.DepartmentId == null && entity.DesignationId == null)
                return (null, null);

            try
            {
                var departmentIds = entity.DepartmentId.HasValue ? new List<long> { entity.DepartmentId.Value } : new List<long>();
                var designationIds = entity.DesignationId.HasValue ? new List<long> { entity.DesignationId.Value } : new List<long>();

                var result = await _coreGrpcClient.GetDepartmentsAndDesignationsAsync(departmentIds, designationIds);
                return (result.Departments.FirstOrDefault()?.Name, result.Designations.FirstOrDefault()?.Name);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to resolve department/designation names from Core service for CandidateProfile {CandidateProfileId}", entity.CandidateProfileId);
                return (null, null);
            }
        }

        private async Task<Domain.Entities.CandidateProfile> GetCurrentProfileEntityAsync()
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            return await _candidateProfileRepository.GetByIdAsync(profileId)
                ?? throw new NotFoundException("CandidateProfile", profileId);
        }

        // ── US-003: Identity-field lock (AC1/AC2/AC4) ──────────────────────
        // Even though JobApplication.CandidateProfileId now links most applications directly, some
        // rows (pre-migration history, or a guest application not yet claimed at registration) can
        // still only be found by CandidateEmail. Email/Phone/NationalId stay locked post-apply so
        // that fallback path never orphans - the FK makes this technically unnecessary for linked
        // rows, but relaxing the lock is a separate decision, not bundled into this change.

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
            var applications = await _jobApplicationRepository.GetByCandidateAsync(entity.CandidateProfileId, entity.Email);
            return applications.Count > 0;
        }

        private static string? NormalizePhoneDigits(string? phone) =>
            phone == null ? null : new string(phone.Where(char.IsDigit).ToArray());
    }
}
