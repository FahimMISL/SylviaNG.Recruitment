using SylviaNG.Recruitment.Application.Features.Payments.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        /// <summary>
        /// Opens (or re-opens, on retry) an SSLCommerz checkout session for a JobApplication that is
        /// AwaitingPayment. This endpoint is [AllowAnonymous] (career-portal applicants have no
        /// guaranteed JWT) and jobApplicationId is a sequential, guessable long, so candidateEmail
        /// - the email the applicant themselves typed at apply time - stands in for an ownership
        /// check. Throws NotFoundException both when the application doesn't exist AND when the
        /// email doesn't match, so this can't be used to enumerate valid application IDs. Returns
        /// Success=false (not an exception) for ordinary "nothing to pay"/"already paid" cases.
        /// </summary>
        Task<PaymentInitiateResponse> InitiateAsync(long jobApplicationId, string candidateEmail);

        /// <summary>
        /// Processes SSLCommerz's server-to-server IPN notification. Always re-validates via
        /// SSLCommerz's Validation API using the val_id - never trusts the raw IPN body's own
        /// status field. Idempotent: safe to call more than once for the same tran_id, and cheap to
        /// do so - an already-Success payment short-circuits before the Validation API call.
        /// Returns the application this tran_id belongs to (and its CandidateEmail) so the
        /// browser-return callbacks can build their redirect without re-reading rows this method
        /// already loaded; null only when the tran_id is unknown.
        /// </summary>
        Task<(long JobApplicationId, string? CandidateEmail)?> HandleIpnAsync(string transactionId, string? validationId, string rawPayload);

        /// <summary>Same email-ownership check as InitiateAsync - see its remarks.</summary>
        Task<PaymentStatusResponse> GetStatusAsync(long jobApplicationId, string candidateEmail);

        /// <summary>Looks up the JobApplicationId (and its CandidateEmail, so the frontend
        /// redirect can carry the same ownership proof Initiate/GetStatus require) for a tran_id -
        /// used by the non-authoritative browser-return callback actions to build the frontend
        /// redirect target.</summary>
        Task<(long JobApplicationId, string? CandidateEmail)?> GetJobApplicationIdByTransactionIdAsync(string transactionId);
    }
}
