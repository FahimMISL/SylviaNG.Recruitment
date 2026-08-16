namespace SylviaNG.Recruitment.Application.Features.ExamVenues.Models
{
    public class ExamVenueResponse
    {
        public long ExamVenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int RoomCount { get; set; }
    }
}
