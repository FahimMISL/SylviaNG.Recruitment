namespace SylviaNG.Recruitment.Application.Common.Settings
{
    /// <summary>
    /// Size/extension policy for candidate profile photo and signature uploads (US-002 AC).
    /// No RootPath here - both reuse the existing IFileStorageService/FileStorageSettings root
    /// (see plan decision), differentiated only by upload sub-folder.
    /// </summary>
    public class CandidatePhotoSignatureSettings
    {
        public const string SectionName = "CandidatePhotoSignature";

        public int MaxFileSizeMB { get; set; } = 2;
        public List<string> AllowedExtensions { get; set; } = new() { ".jpg", ".jpeg", ".png" };
    }
}
