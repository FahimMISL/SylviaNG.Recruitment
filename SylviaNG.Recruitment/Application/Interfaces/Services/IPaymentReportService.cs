using SylviaNG.Recruitment.Application.Features.PaymentReports.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IPaymentReportService
    {
        Task<PaymentTransactionListResponse> GetTransactionsAsync(PagedRequest paging, PaymentTransactionFilterRequest filter);

        Task<ReconciliationResponse> GetReconciliationAsync(ReconciliationRequest request);

        /// <summary>format is "xlsx" or "pdf".</summary>
        Task<PaymentReportFileResponse> ExportReconciliationAsync(ReconciliationRequest request, string format);
    }
}
