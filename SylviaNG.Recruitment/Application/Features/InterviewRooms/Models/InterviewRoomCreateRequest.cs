namespace SylviaNG.Recruitment.Application.Features.InterviewRooms.Models
{
    public class InterviewRoomCreateRequest
    {
        public string RoomName { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}
