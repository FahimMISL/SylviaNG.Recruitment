using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Common.Authorization;
using SylviaNG.Recruitment.Application.Features.PaymentReports.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>
    /// EP-17/US-125/US-128: Finance/HR read-only reporting over the payment data PaymentController
    /// writes. Kept as a SEPARATE controller from PaymentController - that one is deliberately
    /// [AllowAnonymous] end to end for the SSLCommerz callback surface, and mixing that posture
    /// with authorized reporting endpoints on the same controller is easy to get wrong.
    /// EP-15/US-112: gated via RequirePermission (Reports module, View) instead of
    /// [Authorize(Roles="Admin,HR")] - every action here is read-only.
    /// </summary>
    [ApiController]
    [Route("recruitment/payment-report")]
    [RequirePermission(AccessControlModuleEnum.Reports, PermissionActionEnum.View)]
    public class PaymentReportController : ControllerBase
    {
        private readonly IPaymentReportService _paymentReportService;

        public PaymentReportController(IPaymentReportService paymentReportService)
        {
            _paymentReportService = paymentReportService;
        }

        [HttpGet("transactions")]
        public async Task<ActionResult<PaymentTransactionListResponse>> GetTransactions(
            [FromQuery] PagedRequest paging, [FromQuery] PaymentTransactionFilterRequest filter)
        {
            var result = await _paymentReportService.GetTransactionsAsync(paging, filter);
            return Ok(result);
        }

        [HttpGet("reconciliation")]
        public async Task<ActionResult<ReconciliationResponse>> GetReconciliation([FromQuery] ReconciliationRequest request)
        {
            var result = await _paymentReportService.GetReconciliationAsync(request);
            return Ok(result);
        }

        [HttpGet("reconciliation/export")]
        public async Task<IActionResult> ExportReconciliation([FromQuery] ReconciliationRequest request, [FromQuery] string format = "xlsx")
        {
            var file = await _paymentReportService.ExportReconciliationAsync(request, format);
            return File(file.Content, file.ContentType, file.FileName);
        }
    }
}
