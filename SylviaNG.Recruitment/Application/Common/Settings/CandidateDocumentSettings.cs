namespace SylviaNG.Recruitment.Application.Common.Settings
{
    /// <summary>
    /// Size/extension policy for candidate profile document uploads (US-006). No RootPath here -
    /// reuses the existing IFileStorageService/FileStorageSettings root (see plan decision).
    /// </summary>
    public class CandidateDocumentSettings
    {
        public const string SectionName = "CandidateDocuments";

        public int MaxFileSizeMB { get; set; } = 10;
        public List<string> AllowedExtensions { get; set; } = new() { ".pdf", ".jpg", ".jpeg", ".png", ".docx" };
    }
}
