namespace SylviaNG.Recruitment.Application.Features.InterviewRooms.Models
{
    public class InterviewRoomResponse
    {
        public long InterviewRoomId { get; set; }
        public long InterviewVenueId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
    }
}
