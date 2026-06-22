namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Models
{
    public class ExamHallUpdateRequest
    {
        public string? VenueName { get; set; }
        public string? Address { get; set; }
        public string? VirtualLink { get; set; }
        public int? Capacity { get; set; }
        public int? NumberOfRooms { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactPhone { get; set; }
        public bool? IsActive { get; set; }
    }
}
