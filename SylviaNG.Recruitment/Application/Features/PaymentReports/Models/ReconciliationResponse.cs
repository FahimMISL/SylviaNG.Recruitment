namespace SylviaNG.Recruitment.Application.Features.PaymentReports.Models
{
    public class ReconciliationResponse
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int PaidCount { get; set; }
        public decimal PaidAmount { get; set; }
        public int FailedCount { get; set; }
        public int WaivedCount { get; set; }

        /// <summary>Equal to PaidAmount - no refund tracking exists anywhere in this codebase.</summary>
        public decimal NetAmount { get; set; }
    }
}
