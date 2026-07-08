namespace SylviaNG.Recruitment.Application.Common.Settings
{
    /// <summary>
    /// Options bound from the "FileStorage" configuration section.
    /// Backs local-disk attachment storage for job posting supporting documents.
    /// </summary>
    public class FileStorageSettings
    {
        public const string SectionName = "FileStorage";

        public string RootPath { get; set; } = "wwwroot/uploads/job-postings";
        public int MaxFileSizeMB { get; set; } = 10;
        public List<string> AllowedExtensions { get; set; } = new() { ".pdf", ".doc", ".docx" };
    }
}
