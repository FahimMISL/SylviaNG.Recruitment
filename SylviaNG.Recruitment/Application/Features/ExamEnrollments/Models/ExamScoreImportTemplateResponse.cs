namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models
{
    public class ExamScoreImportTemplateResponse
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
