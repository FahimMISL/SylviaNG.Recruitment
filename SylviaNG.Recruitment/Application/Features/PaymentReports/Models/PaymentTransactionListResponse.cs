namespace SylviaNG.Recruitment.Application.Features.PaymentReports.Models
{
    public class PaymentTransactionListResponse
    {
        public List<PaymentTransactionListItemResponse> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        /// <summary>Sum of Amount across every transaction matching the filter, not just the current page.</summary>
        public decimal TotalAmount { get; set; }
    }
}
