namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Models
{
    public class NotificationLogFileResponse
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}
