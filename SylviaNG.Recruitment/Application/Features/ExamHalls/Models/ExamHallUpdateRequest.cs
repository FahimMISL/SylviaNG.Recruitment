namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Models
{
    public class ExamHallUpdateRequest
    {
        public string HallName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int TotalCapacity { get; set; }
        public bool NotifyInvigilatorsOnAssign { get; set; } = true;
        public List<long> InvigilatorEmployeeIds { get; set; } = new();
    }
}
