namespace SylviaNG.Recruitment.Application.Features.CvBank.Models
{
    public class CvBankCvFileResponse
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
