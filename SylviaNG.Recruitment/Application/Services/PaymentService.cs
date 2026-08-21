using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Payments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Externals;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using System.Data;

namespace SylviaNG.Recruitment.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private const string IpnSystemActor = "system:sslcommerz-ipn";

        private readonly IPaymentRepository _paymentRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly ISslCommerzPaymentGateway _gateway;
        private readonly INotificationDispatchQueue _notificationDispatchQueue;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IJobApplicationRepository jobApplicationRepository,
            ISslCommerzPaymentGateway gateway,
            INotificationDispatchQueue notificationDispatchQueue,
            IApplicationSettingService applicationSettingService,
            IUnitOfWork unitOfWork,
            ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _gateway = gateway;
            _notificationDispatchQueue = notificationDispatchQueue;
            _applicationSettingService = applicationSettingService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PaymentInitiateResponse> InitiateAsync(long jobApplicationId, string candidateEmail)
        {
            var jobApplication = await _jobApplicationRepository.GetByIdWithIncludeAsync(
                a => a.JobApplicationId == jobApplicationId,
                a => a.JobPosting)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            if (!EmailMatches(jobApplication.CandidateEmail, candidateEmail))
                throw new NotFoundException("JobApplication", jobApplicationId);

            if (jobApplication.ApplicationStatus == ApplicationStatusEnum.Applied
                || await _paymentRepository.HasSuccessfulPaymentAsync(jobApplicationId))
            {
                return new PaymentInitiateResponse { Success = false, FailureReason = "This application has already been paid for." };
            }

            if (jobApplication.ApplicationStatus != ApplicationStatusEnum.AwaitingPayment)
            {
                return new PaymentInitiateResponse { Success = false, FailureReason = "This application is not awaiting payment." };
            }

            var amount = jobApplication.JobPosting.ApplicationFeeAmount ?? 0;
            var currency = jobApplication.JobPosting.ApplicationFeeCurrency ?? "BDT";

            if (amount <= 0)
            {
                return new PaymentInitiateResponse { Success = false, FailureReason = "No application fee is configured for this vacancy." };
            }

            var transactionId = $"APP{jobApplicationId}-{Guid.NewGuid():N}";

            var sessionResult = await _gateway.InitiateSessionAsync(new SslCommerzSessionRequest(
                transactionId,
                amount,
                currency,
                jobApplication.CandidateName,
                jobApplication.CandidateEmail,
                jobApplication.CandidatePhone,
                $"Application Fee - {jobApplication.JobPosting.Title}"));

            if (!sessionResult.Success)
            {
                _logger.LogWarning("SSLCommerz session-init rejected for JobApplication {JobApplicationId}: {Reason}", jobApplicationId, sessionResult.FailureReason);
                return new PaymentInitiateResponse { Success = false, FailureReason = sessionResult.FailureReason ?? "The payment gateway rejected the request." };
            }

            var payment = new Payment
            {
                JobApplicationId = jobApplicationId,
                Amount = amount,
                Currency = currency,
                TransactionId = transactionId,
                GatewaySessionKey = sessionResult.SessionKey,
                GatewayRedirectUrl = sessionResult.GatewayPageUrl,
                PaymentStatus = PaymentStatusEnum.Initiated
            };

            await _paymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return new PaymentInitiateResponse { Success = true, GatewayRedirectUrl = sessionResult.GatewayPageUrl };
        }

        public async Task<(long JobApplicationId, string? CandidateEmail)?> HandleIpnAsync(string transactionId, string? validationId, string rawPayload)
        {
            var payment = await _paymentRepository.GetByTransactionIdAsync(transactionId);
            if (payment == null)
            {
                _logger.LogWarning("SSLCommerz IPN received for unknown tran_id {TransactionId}.", transactionId);
                return null;
            }

            // Already settled. Both the browser-return callback and SSLCommerz's own async IPN
            // drive this method for the same tran_id (see PaymentController's callback remarks), so
            // without this guard the second one re-ran the whole path: another Validation API call
            // (up to the 10 s HttpClient timeout), another Serializable transaction, and another
            // round of confirmation emails. The transition itself was already idempotent - this
            // makes the *cost* idempotent too.
            if (payment.PaymentStatus == PaymentStatusEnum.Success)
                return await ResolveCallbackTargetAsync(payment.JobApplicationId);

            payment.RawIpnPayload = rawPayload;

            if (string.IsNullOrWhiteSpace(validationId))
            {
                payment.PaymentStatus = PaymentStatusEnum.Failed;
                _paymentRepository.Update(payment);
                await _unitOfWork.SaveChangesAsync();
                return await ResolveCallbackTargetAsync(payment.JobApplicationId);
            }

            SslCommerzValidationResult validation;
            try
            {
                validation = await _gateway.ValidateTransactionAsync(validationId);
            }
            catch (SslCommerzUnavailableException ex)
            {
                // Leave the payment row as-is (still Initiated) so a later IPN retry from
                // SSLCommerz (they redeliver on failure) or a manual status check can re-validate.
                _logger.LogError(ex, "SSLCommerz validation API unreachable while processing IPN for tran_id {TransactionId}.", transactionId);
                return await ResolveCallbackTargetAsync(payment.JobApplicationId);
            }

            var amountMatches = validation.Amount.HasValue && Math.Abs(validation.Amount.Value - payment.Amount) < 0.01m;
            var currencyMatches = string.Equals(validation.Currency, payment.Currency, StringComparison.OrdinalIgnoreCase);
            var tranIdMatches = string.Equals(validation.TransactionId, payment.TransactionId, StringComparison.Ordinal);

            if (!validation.IsValid || !amountMatches || !currencyMatches || !tranIdMatches)
            {
                _logger.LogWarning(
                    "SSLCommerz validation failed or mismatched for tran_id {TransactionId}: IsValid={IsValid}, AmountMatches={AmountMatches}, CurrencyMatches={CurrencyMatches}, TranIdMatches={TranIdMatches}.",
                    transactionId, validation.IsValid, amountMatches, currencyMatches, tranIdMatches);

                payment.PaymentStatus = PaymentStatusEnum.Failed;
                _paymentRepository.Update(payment);
                await _unitOfWork.SaveChangesAsync();
                return await ResolveCallbackTargetAsync(payment.JobApplicationId);
            }

            payment.PaymentStatus = PaymentStatusEnum.Success;
            payment.ValidationId = validationId;
            payment.PaidAt = DateTime.UtcNow;
            _paymentRepository.Update(payment);

            // Serializable so a concurrent caller for the same tran_id (the async IPN and a
            // browser-return callback can both land at once - see PaymentController's callback
            // comments) can't both read AwaitingPayment before either commits and both apply the
            // Applied transition. There's no concurrency token on JobApplication, so plain Read
            // Committed (EF's/UnitOfWork's default) would let a keyed UPDATE from the loser
            // through unnoticed; Serializable makes Postgres detect the read-write conflict and
            // fail the loser with a serialization error (SqlState 40001) at Commit/SaveChanges,
            // which is then treated as a no-op since the winner already recorded the transition.
            await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);

            var jobApplication = await _jobApplicationRepository.GetByIdWithIncludeAsync(
                a => a.JobApplicationId == payment.JobApplicationId,
                a => a.JobPosting);

            var didTransition = jobApplication != null && jobApplication.ApplicationStatus == ApplicationStatusEnum.AwaitingPayment;
            ApplicationStatusEnum fromStatus = default;

            if (jobApplication != null && didTransition)
            {
                fromStatus = jobApplication.ApplicationStatus;
                jobApplication.ApplicationStatus = ApplicationStatusEnum.Applied;

                // Deliberately NOT _jobApplicationRepository.Update(jobApplication): DbSet.Update()
                // marks the root and every reachable tracked entity Modified, and this graph was
                // loaded with .Include(a => a.JobPosting) - so it also issued a full-column UPDATE
                // against JobPostings, taking a row-level write lock on the vacancy for the rest of
                // this Serializable transaction. Every concurrent payment for the same vacancy then
                // serialized on that one row and lost the race with a manufactured 40001, which the
                // handler below treats as "already recorded" - silently dropping real payments.
                // It also rewrote every JobApplications column, ResumeExtractedText (the whole CV
                // body) and CoverLetter included, as TOAST + WAL traffic for unchanged data.
                // The entity is already tracked, so the assignment above is enough: EF writes just
                // ApplicationStatus, and StatusHistory.Add below is still picked up as an insert.

                jobApplication.StatusHistory.Add(new ApplicationStatusHistory
                {
                    JobApplicationId = jobApplication.JobApplicationId,
                    FromStatus = fromStatus,
                    ToStatus = ApplicationStatusEnum.Applied,
                    ChangedByUserName = IpnSystemActor,
                    ChangedAt = DateTime.UtcNow,
                    Note = $"Payment confirmed via SSLCommerz (tran_id: {transactionId})"
                });
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex) when (
                ex is DbUpdateException { InnerException: Npgsql.PostgresException { SqlState: "40001" } }
                or Npgsql.PostgresException { SqlState: "40001" })
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogInformation(
                    "Concurrent IPN/callback for tran_id {TransactionId} lost the race - the other request already recorded this payment.",
                    transactionId);
                return await ResolveCallbackTargetAsync(payment.JobApplicationId);
            }

            // Queued after a successful commit, and only by whichever concurrent request actually
            // won the transition, so a losing retry never sends a duplicate "payment confirmed"
            // email. Candidate action-required email fires at submit time, not here (see
            // JobApplicationService.SubmitAsync) - it must not claim payment is done before it is;
            // this is the actual "you paid, application confirmed" notification.
            //
            // Queued rather than awaited: this method runs on SSLCommerz's browser-return callback,
            // and the applicant's browser is parked on the gateway until it returns. Awaiting the
            // dispatch put a full SMTP session per recipient (candidate + every active HR mailbox,
            // each with 3 bounded retries) between the payment completing and the 302 back to the
            // result page. CompanyId is passed explicitly because this endpoint is [AllowAnonymous]:
            // CompanyScopeMiddleware never runs, so the ambient tenant filter matches every company.
            if (jobApplication != null && didTransition)
            {
                try
                {
                    var placeholders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["CandidateName"] = jobApplication.CandidateName,
                        ["JobPostingTitle"] = jobApplication.JobPosting?.Title ?? string.Empty,
                        ["ApplicationStatus"] = jobApplication.ApplicationStatus.ToString(),
                        ["FromStatus"] = fromStatus.ToString(),
                        ["ToStatus"] = ApplicationStatusEnum.Applied.ToString()
                    };
                    var hrEmail = await _applicationSettingService.GetHrNotificationEmailAsync();
                    var targets = new NotificationDispatchTargets(
                        jobApplication.CandidateEmail,
                        hrEmail,
                        jobApplication.JobApplicationId,
                        NotifyActiveHrUsers: true,
                        CompanyId: jobApplication.CompanyId);

                    _notificationDispatchQueue.TryEnqueue(new NotificationDispatchRequest(
                        RecruitmentEventEnum.ApplicationStatusChanged,
                        placeholders,
                        targets));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error queuing payment-confirmation notification for JobApplicationId {JobApplicationId}.", jobApplication.JobApplicationId);
                }
            }

            return (payment.JobApplicationId, jobApplication?.CandidateEmail);
        }

        /// <summary>Redirect target for the browser-return callbacks. Only called on the paths that
        /// don't already have the JobApplication in hand; the happy path returns the one it loaded.</summary>
        private async Task<(long JobApplicationId, string? CandidateEmail)?> ResolveCallbackTargetAsync(long jobApplicationId)
        {
            var jobApplication = await _jobApplicationRepository.GetByIdAsync(jobApplicationId);
            return (jobApplicationId, jobApplication?.CandidateEmail);
        }

        public async Task<PaymentStatusResponse> GetStatusAsync(long jobApplicationId, string candidateEmail)
        {
            var jobApplication = await _jobApplicationRepository.GetByIdAsync(jobApplicationId)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            if (!EmailMatches(jobApplication.CandidateEmail, candidateEmail))
                throw new NotFoundException("JobApplication", jobApplicationId);

            var payment = await _paymentRepository.GetLatestByJobApplicationIdAsync(jobApplicationId);

            return new PaymentStatusResponse
            {
                JobApplicationId = jobApplicationId,
                ApplicationStatus = jobApplication.ApplicationStatus.ToString(),
                PaymentStatus = payment?.PaymentStatus.ToString(),
                Amount = payment?.Amount,
                Currency = payment?.Currency,
                PaidAt = payment?.PaidAt
            };
        }

        public async Task<(long JobApplicationId, string? CandidateEmail)?> GetJobApplicationIdByTransactionIdAsync(string transactionId)
        {
            var payment = await _paymentRepository.GetByTransactionIdAsync(transactionId);
            if (payment is null)
                return null;

            var jobApplication = await _jobApplicationRepository.GetByIdAsync(payment.JobApplicationId);
            return (payment.JobApplicationId, jobApplication?.CandidateEmail);
        }

        // Empty==empty is treated as a match so JobApplicationService's internal, already-trusted
        // call right after creating the application (passing the just-saved entity's own
        // CandidateEmail back at itself) never gets rejected just because a given apply path left
        // CandidateEmail unset. The check only has teeth when the stored email is non-empty (the
        // normal case, since frontend requires it) - that's the case an anonymous PaymentController
        // caller with a guessed jobApplicationId can't satisfy without knowing the real email.
        private static bool EmailMatches(string? actual, string? candidate) =>
            string.Equals(actual?.Trim() ?? string.Empty, candidate?.Trim() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }
}
