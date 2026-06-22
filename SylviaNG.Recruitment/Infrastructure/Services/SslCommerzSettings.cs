namespace SylviaNG.Recruitment.Infrastructure.Services;

public class SslCommerzSettings
{
    public string StoreId { get; set; } = string.Empty;
    public string StorePassword { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://sandbox.sslcommerz.com";
    public string SuccessUrl { get; set; } = string.Empty;
    public string FailUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
    public string IpnUrl { get; set; } = string.Empty;
}
