namespace SylviaNG.Recruitment.Application.Features.EmergencyContacts.Models
{
    public class EmergencyContactCreateRequest
    {
        public long PreBoardingProfileId { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? AlternatePhone { get; set; }
        public string? Address { get; set; }
        public bool IsPrimary { get; set; }
    }
}
