namespace SylviaNG.Recruitment.Application.Features.ExamRooms.Models
{
    public class ExamRoomCreateRequest
    {
        public string RoomName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int RequiredInvigilatorCount { get; set; } = 0;
    }
}
