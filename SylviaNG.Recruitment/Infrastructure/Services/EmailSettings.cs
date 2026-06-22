namespace SylviaNG.Recruitment.Infrastructure.Services;

public class EmailSettings
{
    public string SmtpHost { get; set; } = "smtp.gmail.com";
    public int SmtpPort { get; set; } = 587;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderDisplayEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = "SylviaNG Recruitment";
    public string AppPassword { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public string HrNotificationEmail { get; set; } = string.Empty;
}
