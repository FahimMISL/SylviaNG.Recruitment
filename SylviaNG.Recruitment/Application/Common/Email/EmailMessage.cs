namespace SylviaNG.Recruitment.Application.Common.Email
{
    public class EmailMessage
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string HtmlBody { get; set; } = string.Empty;
        public List<EmailAttachment> Attachments { get; set; } = new();
    }

    public class EmailAttachment
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
    }

    public class EmailSendResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
