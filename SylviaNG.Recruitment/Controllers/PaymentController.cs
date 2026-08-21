using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.Payments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>
    /// SSLCommerz payment endpoints (EP-17). All actions are [AllowAnonymous]: career-portal
    /// applicants have no guaranteed JWT (CareerPortalController itself is anonymous), and
    /// SSLCommerz's own IPN/browser-return callbacks obviously carry no application JWT either.
    /// Matches the app's existing risk posture - JobApplicationController.GetById already has no
    /// ownership check by ID today; the IPN handler is the only money-moving path and IS properly
    /// protected via SSLCommerz's Validation API cross-check (see PaymentService.HandleIpnAsync).
    /// </summary>
    [ApiController]
    [Route("recruitment/payment")]
    [AllowAnonymous]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly SslCommerzSettings _settings;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, IOptions<SslCommerzSettings> settings, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _settings = settings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Opens (or re-opens, after a failed/cancelled attempt) an SSLCommerz checkout session
        /// for an application awaiting payment. candidateEmail (the email the applicant typed at
        /// apply time) stands in for an ownership check, since jobApplicationId alone is a
        /// sequential, guessable long and this action is [AllowAnonymous] - see IPaymentService.
        /// </summary>
        [HttpPost("initiate/{jobApplicationId}")]
        public async Task<ActionResult<PaymentInitiateResponse>> Initiate(long jobApplicationId, [FromQuery] string candidateEmail)
        {
            var result = await _paymentService.InitiateAsync(jobApplicationId, candidateEmail);
            return Ok(result);
        }

        /// <summary>
        /// Latest payment status for an application, polled by the frontend payment-result page
        /// until the IPN handler resolves it to Success/Failed. Same email-ownership check as
        /// Initiate.
        /// </summary>
        [HttpGet("status/{jobApplicationId}")]
        public async Task<ActionResult<PaymentStatusResponse>> GetStatus(long jobApplicationId, [FromQuery] string candidateEmail)
        {
            var result = await _paymentService.GetStatusAsync(jobApplicationId, candidateEmail);
            return Ok(result);
        }

        /// <summary>
        /// SSLCommerz's authoritative server-to-server notification. Re-validates via their
        /// Validation API rather than trusting this POST body's own status field - see
        /// PaymentService.HandleIpnAsync. Idempotent, so it's harmless if the browser-return
        /// callbacks below already resolved the same tran_id first.
        /// </summary>
        [HttpPost("ipn")]
        public async Task<ActionResult> Ipn([FromForm] IFormCollection form)
        {
            var (tranId, valId, rawPayload) = ExtractIpnFields(form);

            if (string.IsNullOrWhiteSpace(tranId))
            {
                _logger.LogWarning("SSLCommerz IPN received with no tran_id.");
                return Ok();
            }

            await _paymentService.HandleIpnAsync(tranId, valId, rawPayload);

            // SSLCommerz only expects a 200 OK acknowledgement, no body contract.
            return Ok();
        }

        /// <summary>
        /// Browser-return targets - SSLCommerz redirects the candidate's browser here after
        /// checkout. These carry the same tran_id/val_id as the async IPN, so each one drives the
        /// SAME validated confirmation (PaymentService.HandleIpnAsync - still re-checks via
        /// SSLCommerz's own Validation API, never trusts this request's own status/val_id blindly)
        /// rather than waiting on the IPN alone: the IPN can be delayed, rate-limited, or (for
        /// some sandbox/demo store configurations) never delivered at all if it isn't registered
        /// in the merchant panel, whereas the browser redirect always happens. If the real IPN
        /// does also arrive later, HandleIpnAsync's idempotent update makes it a harmless no-op.
        /// </summary>
        [HttpPost("callback/success")]
        public async Task<ActionResult> CallbackSuccess([FromForm] IFormCollection form) => await ConfirmAndRedirectToFrontendResult(form, "success");

        [HttpPost("callback/fail")]
        public async Task<ActionResult> CallbackFail([FromForm] IFormCollection form) => await ConfirmAndRedirectToFrontendResult(form, "fail");

        [HttpPost("callback/cancel")]
        public async Task<ActionResult> CallbackCancel([FromForm] IFormCollection form) => await ConfirmAndRedirectToFrontendResult(form, "cancel");

        private async Task<ActionResult> ConfirmAndRedirectToFrontendResult(IFormCollection form, string status)
        {
            var (tranId, valId, rawPayload) = ExtractIpnFields(form);

            // HandleIpnAsync already loads both the Payment and its JobApplication, so it hands the
            // redirect target back directly - this used to re-read both rows through
            // GetJobApplicationIdByTransactionIdAsync while the applicant's browser sat waiting.
            (long JobApplicationId, string? CandidateEmail)? application = null;
            if (!string.IsNullOrWhiteSpace(tranId))
            {
                application = await _paymentService.HandleIpnAsync(tranId, valId, rawPayload);
            }

            var frontendBase = _settings.FrontendReturnBaseUrl.TrimEnd('/');

            // candidateEmail rides along here (backend-resolved, not attacker-suppliable) so the
            // frontend payment-result page can pass it straight back on its status poll - it has
            // no other way to know it, since this redirect is the first time that page runs.
            var query = application.HasValue
                ? $"?jobApplicationId={application.Value.JobApplicationId}&status={status}&candidateEmail={Uri.EscapeDataString(application.Value.CandidateEmail ?? string.Empty)}"
                : $"?status={status}";

            return Redirect($"{frontendBase}/careers/payment-result{query}");
        }

        private static (string TranId, string? ValId, string RawPayload) ExtractIpnFields(IFormCollection form)
        {
            var tranId = form["tran_id"].ToString();
            var valId = form["val_id"].ToString();
            var rawPayload = string.Join('&', form.Select(kv => $"{kv.Key}={kv.Value}"));
            return (tranId, string.IsNullOrWhiteSpace(valId) ? null : valId, rawPayload);
        }
    }
}
