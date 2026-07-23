using SylviaNG.Recruitment.Application.Features.Payments.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        /// <summary>
        /// Opens (or re-opens, on retry) an SSLCommerz checkout session for a JobApplication that is
        /// AwaitingPayment. Throws NotFoundException if the application doesn't exist; returns
        /// Success=false (not an exception) for ordinary "nothing to pay"/"already paid" cases.
        /// </summary>
        Task<PaymentInitiateResponse> InitiateAsync(long jobApplicationId);

        /// <summary>
        /// Processes SSLCommerz's server-to-server IPN notification. Always re-validates via
        /// SSLCommerz's Validation API using the val_id - never trusts the raw IPN body's own
        /// status field. Idempotent: safe to call more than once for the same tran_id.
        /// </summary>
        Task HandleIpnAsync(string transactionId, string? validationId, string rawPayload);

        Task<PaymentStatusResponse> GetStatusAsync(long jobApplicationId);

        /// <summary>Looks up the JobApplicationId for a tran_id - used by the non-authoritative
        /// browser-return callback actions to build the frontend redirect target.</summary>
        Task<long?> GetJobApplicationIdByTransactionIdAsync(string transactionId);
    }
}
