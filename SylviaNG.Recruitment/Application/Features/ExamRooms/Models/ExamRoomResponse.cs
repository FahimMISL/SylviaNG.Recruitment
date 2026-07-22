namespace SylviaNG.Recruitment.Application.Features.ExamRooms.Models
{
    public class ExamRoomResponse
    {
        public long ExamRoomId { get; set; }
        public long ExamVenueId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int RequiredInvigilatorCount { get; set; }
        public bool IsActive { get; set; }
    }
}
