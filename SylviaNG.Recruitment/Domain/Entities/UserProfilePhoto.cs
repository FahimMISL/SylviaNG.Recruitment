using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class UserProfilePhoto : Audit
{
    public long UserProfilePhotoId { get; set; }
    public string KeycloakUserId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public byte[] PhotoData { get; set; } = Array.Empty<byte>();
    public bool IsActive { get; set; } = true;
}
