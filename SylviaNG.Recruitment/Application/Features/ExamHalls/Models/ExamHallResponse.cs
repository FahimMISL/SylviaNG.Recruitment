namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Models
{
    public class ExamHallResponse
    {
        public long ExamHallId { get; set; }
        public string HallName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int TotalCapacity { get; set; }
        public bool NotifyInvigilatorsOnAssign { get; set; }
        public bool IsActive { get; set; }
        public List<long> InvigilatorEmployeeIds { get; set; } = new();
    }
}
