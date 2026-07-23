namespace SylviaNG.Recruitment.Application.Common.Settings
{
    /// <summary>
    /// Options bound from the "ExamQuestionImport" configuration section.
    /// Backs the US-054 bulk-import file-upload size/extension policy.
    /// </summary>
    public class ExamQuestionImportSettings
    {
        public const string SectionName = "ExamQuestionImport";

        public int MaxFileSizeMB { get; set; } = 5;
        public List<string> AllowedExtensions { get; set; } = new() { ".xlsx", ".csv" };
    }
}
