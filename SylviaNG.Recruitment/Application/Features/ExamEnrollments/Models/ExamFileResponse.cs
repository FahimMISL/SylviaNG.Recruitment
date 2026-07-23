namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models
{
    /// <summary>Shared shape for the seat-plan PDF/Excel and admit-card PDF downloads. Same convention as ExamQuestionImportTemplateResponse.</summary>
    public class ExamFileResponse
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
