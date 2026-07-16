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
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICoreGrpcClient _coreGrpcClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CandidateProfileService> _logger;

        public CandidateProfileService(
            ICandidateProfileRepository candidateProfileRepository,
            IJobApplicationRepository jobApplicationRepository,
            ICurrentCandidateService currentCandidateService,
            IFileStorageService fileStorageService,
            ICoreGrpcClient coreGrpcClient,
            IUnitOfWork unitOfWork,
            ILogger<CandidateProfileService> logger)
        {
            _candidateProfileRepository = candidateProfileRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _currentCandidateService = currentCandidateService;
            _fileStorageService = fileStorageService;
            _coreGrpcClient = coreGrpcClient;
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
            (response.DepartmentName, response.DesignationName) = await ResolveOrgNamesAsync(entity);
            return response;
        }

        public async Task<PagedResult<CandidateProfileSummaryResponse>> GetPagedAsync(PagedRequest request)
        {
            var pagedResult = await _candidateProfileRepository.GetPagedAsync(request);

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

            var response = entity.ToDetailResponse(applications);
            (response.DepartmentName, response.DesignationName) = await ResolveOrgNamesAsync(entity);
            return response;
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

            entity.ApplyPersonalInfoUpdate(request);
            entity.UpdatedAt = DateTime.UtcNow;

            _candidateProfileRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateContactAsync(CandidateProfileContactUpdateRequest request)
        {
            var entity = await GetCurrentProfileEntityAsync();

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
    }
}
