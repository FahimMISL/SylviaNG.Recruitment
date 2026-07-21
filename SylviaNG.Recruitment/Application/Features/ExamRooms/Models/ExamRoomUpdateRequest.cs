namespace SylviaNG.Recruitment.Application.Features.ExamRooms.Models
{
    public class ExamRoomUpdateRequest
    {
        public string RoomName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public bool NotifyInvigilatorsOnAssign { get; set; } = true;
        public List<long> InvigilatorEmployeeIds { get; set; } = new();
    }
}
