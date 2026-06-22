namespace SylviaNG.Recruitment.Application.Features.UserProfilePhotos.Models;

public class UserProfilePhotoResponse
{
    public long UserProfilePhotoId { get; set; }
    public string KeycloakUserId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
