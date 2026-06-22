namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Models
{
    public class InterviewVenueResponse
    {
        public long InterviewVenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public int? Capacity { get; set; }
        public string? EquipmentDetails { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPhone { get; set; }
        public bool IsActive { get; set; }
    }
}
