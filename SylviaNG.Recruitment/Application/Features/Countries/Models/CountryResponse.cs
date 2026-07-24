namespace SylviaNG.Recruitment.Application.Features.Countries.Models
{
    public class CountryResponse
    {
        public long CountryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string DialCode { get; set; } = string.Empty;
    }
}
