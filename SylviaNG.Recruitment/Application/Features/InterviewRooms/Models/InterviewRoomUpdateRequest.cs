namespace SylviaNG.Recruitment.Application.Features.InterviewRooms.Models
{
    public class InterviewRoomUpdateRequest
    {
        public string RoomName { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}
