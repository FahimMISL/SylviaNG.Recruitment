using SylviaNG.Recruitment.Application.Features.UserProfilePhotos.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings;

public static class UserProfilePhotoMapper
{
    public static UserProfilePhotoResponse ToResponse(this UserProfilePhoto entity)
    {
        return new UserProfilePhotoResponse
        {
            UserProfilePhotoId = entity.UserProfilePhotoId,
            KeycloakUserId = entity.KeycloakUserId,
            FileName = entity.FileName,
            ContentType = entity.ContentType,
            FileSizeBytes = entity.FileSizeBytes,
            UpdatedAt = entity.UpdatedAt,
        };
    }
}
