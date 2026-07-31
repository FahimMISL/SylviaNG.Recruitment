using ClosedXML.Excel;
using SylviaNG.Recruitment.Application.Features.PaymentReports.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Services
{
    public class PaymentReportService : IPaymentReportService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IPaymentReportPdfGeneratorService _pdfGeneratorService;

        public PaymentReportService(
            IPaymentRepository paymentRepository,
            IJobApplicationRepository jobApplicationRepository,
            IPaymentReportPdfGeneratorService pdfGeneratorService)
        {
            _paymentRepository = paymentRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _pdfGeneratorService = pdfGeneratorService;
        }

        public async Task<PaymentTransactionListResponse> GetTransactionsAsync(PagedRequest paging, PaymentTransactionFilterRequest filter)
        {
            var paged = await _paymentRepository.GetPagedTransactionsAsync(filter, paging);
            var totalAmount = await _paymentRepository.GetFilteredTotalAmountAsync(filter);

            return new PaymentTransactionListResponse
            {
                Data = paged.Data.Select(p => new PaymentTransactionListItemResponse
                {
                    PaymentId = p.PaymentId,
                    JobApplicationId = p.JobApplicationId,
                    CandidateName = p.JobApplication.CandidateName,
                    CandidateEmail = p.JobApplication.CandidateEmail,
                    VacancyTitle = p.JobApplication.JobPosting.Title,
                    PaymentStatus = p.PaymentStatus,
                    Amount = p.Amount,
                    Currency = p.Currency,
                    TransactionId = p.TransactionId,
                    PaidAt = p.PaidAt
                }).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalAmount = totalAmount
            };
        }

        public async Task<ReconciliationResponse> GetReconciliationAsync(ReconciliationRequest request)
        {
            var latestPayments = await _paymentRepository.GetLatestPaymentsInScopeAsync(
                request.DateFrom, request.DateTo, request.JobPostingId, request.DepartmentId, request.SiteId);

            var paidPayments = latestPayments.Where(p => p.PaymentStatus == PaymentStatusEnum.Success).ToList();
            var failedCount = latestPayments.Count(p => p.PaymentStatus is PaymentStatusEnum.Failed or PaymentStatusEnum.Cancelled);

            var waivedCount = await _jobApplicationRepository.CountWaivedInPeriodAsync(
                request.DateFrom, request.DateTo, request.JobPostingId, request.DepartmentId, request.SiteId);

            var paidAmount = paidPayments.Sum(p => p.Amount);

            return new ReconciliationResponse
            {
                DateFrom = request.DateFrom,
                DateTo = request.DateTo,
                PaidCount = paidPayments.Count,
                PaidAmount = paidAmount,
                FailedCount = failedCount,
                WaivedCount = waivedCount,
                NetAmount = paidAmount
            };
        }

        public async Task<PaymentReportFileResponse> ExportReconciliationAsync(ReconciliationRequest request, string format)
        {
            var summary = await GetReconciliationAsync(request);
            var stamp = $"{request.DateFrom:yyyyMMdd}-{request.DateTo:yyyyMMdd}";

            if (string.Equals(format, "pdf", StringComparison.OrdinalIgnoreCase))
            {
                var pdfBytes = await _pdfGeneratorService.GenerateReconciliationPdfAsync(request, summary);
                return new PaymentReportFileResponse
                {
                    Content = pdfBytes,
                    ContentType = "application/pdf",
                    FileName = $"Reconciliation-Report-{stamp}.pdf"
                };
            }

            return new PaymentReportFileResponse
            {
                Content = BuildExcel(summary),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = $"Reconciliation-Report-{stamp}.xlsx"
            };
        }

        private static byte[] BuildExcel(ReconciliationResponse summary)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Reconciliation");

            sheet.Cell(1, 1).Value = "Period";
            sheet.Cell(1, 2).Value = $"{summary.DateFrom:yyyy-MM-dd} to {summary.DateTo:yyyy-MM-dd}";

            sheet.Cell(3, 1).Value = "Metric";
            sheet.Cell(3, 2).Value = "Count";
            sheet.Cell(3, 3).Value = "Amount";
            sheet.Row(3).Style.Font.Bold = true;

            sheet.Cell(4, 1).Value = "Paid";
            sheet.Cell(4, 2).Value = summary.PaidCount;
            sheet.Cell(4, 3).Value = summary.PaidAmount;

            sheet.Cell(5, 1).Value = "Failed";
            sheet.Cell(5, 2).Value = summary.FailedCount;
            sheet.Cell(5, 3).Value = 0;

            sheet.Cell(6, 1).Value = "Waived";
            sheet.Cell(6, 2).Value = summary.WaivedCount;
            sheet.Cell(6, 3).Value = 0;

            sheet.Cell(7, 1).Value = "Net";
            sheet.Cell(7, 2).Value = string.Empty;
            sheet.Cell(7, 3).Value = summary.NetAmount;
            sheet.Row(7).Style.Font.Bold = true;

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
