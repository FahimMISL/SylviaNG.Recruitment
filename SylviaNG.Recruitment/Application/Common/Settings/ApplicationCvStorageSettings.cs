namespace SylviaNG.Recruitment.Application.Common.Settings
{
    /// <summary>
    /// Options bound from the "FileStorage:Applications" configuration section.
    /// Backs local-disk CV storage for career-portal / internal-job-board job applications.
    /// Kept separate from <see cref="FileStorageSettings"/> (job posting attachments) so the
    /// two storage areas can be configured and evolve independently.
    /// </summary>
    public class ApplicationCvStorageSettings
    {
        public const string SectionName = "FileStorage:Applications";

        public string RootPath { get; set; } = "wwwroot/uploads/applications";
        public int MaxFileSizeMB { get; set; } = 10;
        public List<string> AllowedExtensions { get; set; } = new() { ".pdf", ".doc", ".docx" };
    }
}
