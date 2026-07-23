using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Payments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Externals;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private const string IpnSystemActor = "system:sslcommerz-ipn";

        private readonly IPaymentRepository _paymentRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly ISslCommerzPaymentGateway _gateway;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IJobApplicationRepository jobApplicationRepository,
            ISslCommerzPaymentGateway gateway,
            IUnitOfWork unitOfWork,
            ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _gateway = gateway;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PaymentInitiateResponse> InitiateAsync(long jobApplicationId)
        {
            var jobApplication = await _jobApplicationRepository.GetByIdWithIncludeAsync(
                a => a.JobApplicationId == jobApplicationId,
                a => a.JobPosting)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

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

        public async Task HandleIpnAsync(string transactionId, string? validationId, string rawPayload)
        {
            var payment = await _paymentRepository.GetByTransactionIdAsync(transactionId);
            if (payment == null)
            {
                _logger.LogWarning("SSLCommerz IPN received for unknown tran_id {TransactionId}.", transactionId);
                return;
            }

            payment.RawIpnPayload = rawPayload;

            if (string.IsNullOrWhiteSpace(validationId))
            {
                payment.PaymentStatus = PaymentStatusEnum.Failed;
                _paymentRepository.Update(payment);
                await _unitOfWork.SaveChangesAsync();
                return;
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
                return;
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
                return;
            }

            payment.PaymentStatus = PaymentStatusEnum.Success;
            payment.ValidationId = validationId;
            payment.PaidAt = DateTime.UtcNow;
            _paymentRepository.Update(payment);

            var jobApplication = await _jobApplicationRepository.GetByIdAsync(payment.JobApplicationId);
            if (jobApplication != null && jobApplication.ApplicationStatus == ApplicationStatusEnum.AwaitingPayment)
            {
                var fromStatus = jobApplication.ApplicationStatus;
                jobApplication.ApplicationStatus = ApplicationStatusEnum.Applied;
                _jobApplicationRepository.Update(jobApplication);

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

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PaymentStatusResponse> GetStatusAsync(long jobApplicationId)
        {
            var jobApplication = await _jobApplicationRepository.GetByIdAsync(jobApplicationId)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

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

        public async Task<long?> GetJobApplicationIdByTransactionIdAsync(string transactionId)
        {
            var payment = await _paymentRepository.GetByTransactionIdAsync(transactionId);
            return payment?.JobApplicationId;
        }
    }
}
