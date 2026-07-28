namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Models
{
    public class InterviewVenueResponse
    {
        public long InterviewVenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int RoomCount { get; set; }
    }
}
