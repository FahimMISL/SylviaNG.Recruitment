namespace SylviaNG.Recruitment.Application.Features.PaymentReports.Models
{
    public class PaymentReportFileResponse
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}
