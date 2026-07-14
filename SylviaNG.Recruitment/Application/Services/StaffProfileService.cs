using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;
using System.Security.Claims;

namespace SylviaNG.Recruitment.Application.Services
{
    public class StaffProfileService : IStaffProfileService
    {
        private readonly IStaffProfileRepository _staffProfileRepository;
        private readonly ICurrentCandidateService _currentUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;

        public StaffProfileService(
            IStaffProfileRepository staffProfileRepository,
            ICurrentCandidateService currentUserService,
            IHttpContextAccessor httpContextAccessor,
            IFileStorageService fileStorageService,
            IUnitOfWork unitOfWork)
        {
            _staffProfileRepository = staffProfileRepository;
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;
            _fileStorageService = fileStorageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<string?> GetMyPhotoPathAsync()
        {
            var entity = await GetOrCreateCurrentEntityAsync();
            return entity.ProfilePhotoPath;
        }

        public async Task<string> UploadPhotoAsync(IFormFile file)
        {
            var entity = await GetOrCreateCurrentEntityAsync();
            var oldFilePath = entity.ProfilePhotoPath;

            await using var stream = file.OpenReadStream();
            var (_, filePath) = await _fileStorageService.SaveAsync(stream, file.FileName, $"staff-photos/{entity.StaffProfileId}");

            entity.ProfilePhotoPath = filePath;
            entity.UpdatedAt = DateTime.UtcNow;

            _staffProfileRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrEmpty(oldFilePath))
                await _fileStorageService.DeleteAsync(oldFilePath);

            return filePath;
        }

        public async Task DeletePhotoAsync()
        {
            var entity = await GetOrCreateCurrentEntityAsync();
            if (string.IsNullOrEmpty(entity.ProfilePhotoPath))
                return;

            var filePath = entity.ProfilePhotoPath;
            entity.ProfilePhotoPath = null;
            entity.UpdatedAt = DateTime.UtcNow;

            _staffProfileRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            await _fileStorageService.DeleteAsync(filePath);
        }

        // Reuses ICurrentCandidateService.GetCurrentKeycloakSubjectId() - that method is
        // role-agnostic despite its name (just reads ClaimTypes.NameIdentifier/"sub").
        private async Task<StaffProfile> GetOrCreateCurrentEntityAsync()
        {
            var subjectId = _currentUserService.GetCurrentKeycloakSubjectId();

            var existing = await _staffProfileRepository.GetByKeycloakSubjectIdAsync(subjectId);
            if (existing != null)
                return existing;

            var user = _httpContextAccessor.HttpContext!.User;
            var fullName = user.FindFirst(ClaimTypes.Name)?.Value ?? user.FindFirst("name")?.Value ?? string.Empty;

            var entity = new StaffProfile
            {
                KeycloakSubjectId = subjectId,
                FullName = fullName
            };

            await _staffProfileRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity;
        }
    }
}
