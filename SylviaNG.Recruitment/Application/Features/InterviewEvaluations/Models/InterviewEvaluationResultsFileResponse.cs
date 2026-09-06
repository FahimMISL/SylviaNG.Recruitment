namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models
{
    public class InterviewEvaluationResultsFileResponse
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}
