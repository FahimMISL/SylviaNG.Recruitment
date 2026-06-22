namespace SylviaNG.Recruitment.Application.Features.EmergencyContacts.Models
{
    public class EmergencyContactUpdateRequest
    {
        public long? PreBoardingProfileId { get; set; }
        public string? ContactName { get; set; }
        public string? Relationship { get; set; }
        public string? Phone { get; set; }
        public string? AlternatePhone { get; set; }
        public string? Address { get; set; }
        public bool? IsPrimary { get; set; }
        public bool? IsActive { get; set; }
    }
}
