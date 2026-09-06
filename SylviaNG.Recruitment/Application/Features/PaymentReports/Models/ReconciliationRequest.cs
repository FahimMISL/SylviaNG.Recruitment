namespace SylviaNG.Recruitment.Application.Features.PaymentReports.Models
{
    public class ReconciliationRequest
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public long? JobPostingId { get; set; }
        public long? DepartmentId { get; set; }
    }
}
