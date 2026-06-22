using SylviaNG.Recruitment.Application.Features.UserProfilePhotos.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services;

public class UserProfilePhotoService : IUserProfilePhotoService
{
    private readonly IUserProfilePhotoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UserProfilePhotoService(IUserProfilePhotoRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserProfilePhotoResponse?> GetByKeycloakUserIdAsync(string keycloakUserId)
    {
        var entity = await _repository.GetByKeycloakUserIdAsync(keycloakUserId);
        return entity?.ToResponse();
    }

    public async Task<long> UploadAsync(string keycloakUserId, string fileName, string contentType, byte[] photoData)
    {
        var existing = await _repository.GetByKeycloakUserIdAsync(keycloakUserId);

        if (existing != null)
        {
            existing.FileName = fileName;
            existing.ContentType = contentType;
            existing.FileSizeBytes = photoData.Length;
            existing.PhotoData = photoData;
            _repository.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return existing.UserProfilePhotoId;
        }

        var entity = new UserProfilePhoto
        {
            KeycloakUserId = keycloakUserId,
            FileName = fileName,
            ContentType = contentType,
            FileSizeBytes = photoData.Length,
            PhotoData = photoData,
            IsActive = true,
        };

        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.UserProfilePhotoId;
    }

    public async Task DeleteAsync(string keycloakUserId)
    {
        var entity = await _repository.GetByKeycloakUserIdAsync(keycloakUserId)
            ?? throw new KeyNotFoundException("Profile photo not found.");
        _repository.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
