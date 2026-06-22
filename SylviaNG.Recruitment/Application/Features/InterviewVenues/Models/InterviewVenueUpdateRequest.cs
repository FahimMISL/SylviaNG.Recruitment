namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Models
{
    public class InterviewVenueUpdateRequest
    {
        public string? VenueName { get; set; }
        public string? Address { get; set; }
        public int? Capacity { get; set; }
        public string? EquipmentDetails { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPhone { get; set; }
        public bool? IsActive { get; set; }
    }
}
