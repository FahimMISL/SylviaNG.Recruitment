using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Services;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Controllers;

[ApiController]
[Route("recruitment/payment")]
public class PaymentController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISslCommerzService _sslCommerz;
    private readonly ILogger<PaymentController> _logger;
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;

    public PaymentController(IUnitOfWork unitOfWork, ISslCommerzService sslCommerz, ILogger<PaymentController> logger,
        IEmailService emailService, IOptions<EmailSettings> emailSettings)
    {
        _unitOfWork = unitOfWork;
        _sslCommerz = sslCommerz;
        _logger = logger;
        _emailService = emailService;
        _emailSettings = emailSettings.Value;
    }

    [Authorize(Roles = "Candidate")]
    [EnableRateLimiting("sensitive")]
    [HttpPost("initiate")]
    public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentRequest request)
    {
        var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(keycloakUserId)) return Unauthorized();

        var candidate = await _unitOfWork.Context.Set<Candidate>()
            .FirstOrDefaultAsync(c => c.KeycloakUserId == keycloakUserId);
        if (candidate == null) return Unauthorized();

        var fee = await _unitOfWork.Context.Set<ApplicationFee>()
            .FirstOrDefaultAsync(f => f.JobPostingId == request.JobPostingId && f.IsActive);
        if (fee == null) return BadRequest("No application fee configured for this job.");

        var alreadyPaid = await _unitOfWork.Context.Set<PaymentTransaction>()
            .AnyAsync(t => t.CandidateId == candidate.CandidateId
                && t.JobPostingId == request.JobPostingId
                && t.PaymentStatus == PaymentStatusEnum.Completed
                && t.IsActive);
        if (alreadyPaid) return BadRequest("You have already paid for this job posting.");

        var transactionId = $"TXN_{candidate.CandidateId}_{request.JobPostingId}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..8]}";

        var job = await _unitOfWork.Context.Set<JobPosting>()
            .FirstOrDefaultAsync(j => j.JobPostingId == request.JobPostingId);

        var (success, gatewayUrl, sessionKey) = await _sslCommerz.InitiatePaymentAsync(
            transactionId, fee.Amount, fee.Currency,
            candidate.FullName ?? "Candidate",
            candidate.Email ?? "noemail@example.com",
            candidate.Phone ?? "01700000000",
            $"Application Fee - {job?.Title ?? "Job"}");

        if (!success) return BadRequest("Failed to initiate payment. Please try again.");

        var transaction = new PaymentTransaction
        {
            TransactionId = transactionId,
            SessionKey = sessionKey,
            CandidateId = candidate.CandidateId,
            JobPostingId = request.JobPostingId,
            ApplicationFeeId = fee.ApplicationFeeId,
            Amount = fee.Amount,
            Currency = fee.Currency,
            PaymentStatus = PaymentStatusEnum.Processing,
            IsActive = true
        };

        await _unitOfWork.Context.Set<PaymentTransaction>().AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { gatewayUrl, transactionId });
    }

    [AllowAnonymous]
    [HttpPost("ipn")]
    public async Task<IActionResult> IpnCallback()
    {
        var formData = new Dictionary<string, string>();
        foreach (var key in Request.Form.Keys)
        {
            formData[key] = Request.Form[key].ToString();
        }

        _logger.LogInformation("IPN received: tran_id={TranId}, status={Status}",
            formData.GetValueOrDefault("tran_id"), formData.GetValueOrDefault("status"));

        if (!_sslCommerz.VerifyIpnHash(formData))
        {
            _logger.LogWarning("IPN hash verification failed for tran_id={TranId}", formData.GetValueOrDefault("tran_id"));
            return BadRequest("Invalid IPN signature.");
        }

        var tranId = formData.GetValueOrDefault("tran_id") ?? "";
        var status = formData.GetValueOrDefault("status") ?? "";
        var valId = formData.GetValueOrDefault("val_id") ?? "";

        var transaction = await _unitOfWork.Context.Set<PaymentTransaction>()
            .FirstOrDefaultAsync(t => t.TransactionId == tranId && t.IsActive);

        if (transaction == null)
        {
            _logger.LogWarning("IPN: Transaction not found: {TranId}", tranId);
            return NotFound();
        }

        if (transaction.PaymentStatus == PaymentStatusEnum.Completed)
            return Ok("Already processed.");

        if (status == "VALID")
        {
            var paidAmountStr = formData.GetValueOrDefault("amount") ?? "0";
            if (!decimal.TryParse(paidAmountStr, out var paidAmount) || paidAmount != transaction.Amount)
            {
                _logger.LogWarning("IPN: Amount mismatch for {TranId}. Expected={Expected}, Received={Received}",
                    tranId, transaction.Amount, paidAmountStr);
                transaction.PaymentStatus = PaymentStatusEnum.Failed;
                transaction.GatewayResponse = System.Text.Json.JsonSerializer.Serialize(formData);
                await _unitOfWork.SaveChangesAsync();
                return BadRequest("Amount mismatch.");
            }

            var paidCurrency = formData.GetValueOrDefault("currency_type") ?? formData.GetValueOrDefault("currency") ?? "";
            if (!string.Equals(paidCurrency, transaction.Currency, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("IPN: Currency mismatch for {TranId}. Expected={Expected}, Received={Received}",
                    tranId, transaction.Currency, paidCurrency);
                transaction.PaymentStatus = PaymentStatusEnum.Failed;
                transaction.GatewayResponse = System.Text.Json.JsonSerializer.Serialize(formData);
                await _unitOfWork.SaveChangesAsync();
                return BadRequest("Currency mismatch.");
            }

            var (valid, validationStatus) = await _sslCommerz.ValidatePaymentAsync(valId);
            if (valid)
            {
                transaction.PaymentStatus = PaymentStatusEnum.Completed;
                transaction.PaidAt = DateTime.UtcNow;
                transaction.BankTransactionId = formData.GetValueOrDefault("bank_tran_id");
                transaction.CardType = formData.GetValueOrDefault("card_type");
            }
            else
            {
                transaction.PaymentStatus = PaymentStatusEnum.Failed;
                _logger.LogWarning("IPN: Validation failed for {TranId}, status={VStatus}", tranId, validationStatus);
            }
        }
        else
        {
            transaction.PaymentStatus = PaymentStatusEnum.Failed;
        }

        transaction.GatewayResponse = System.Text.Json.JsonSerializer.Serialize(formData);
        await _unitOfWork.SaveChangesAsync();

        return Ok("IPN processed.");
    }

    [Authorize(Roles = "Candidate")]
    [HttpGet("verify/{transactionId}")]
    public async Task<IActionResult> VerifyPayment(string transactionId)
    {
        var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(keycloakUserId)) return Unauthorized();

        var candidate = await _unitOfWork.Context.Set<Candidate>()
            .FirstOrDefaultAsync(c => c.KeycloakUserId == keycloakUserId);
        if (candidate == null) return Unauthorized();

        var transaction = await _unitOfWork.Context.Set<PaymentTransaction>()
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId
                && t.CandidateId == candidate.CandidateId
                && t.IsActive);

        if (transaction == null) return NotFound();

        return Ok(new
        {
            transaction.TransactionId,
            transaction.PaymentStatus,
            transaction.Amount,
            transaction.Currency,
            transaction.PaidAt,
            transaction.CardType
        });
    }

    [Authorize(Roles = "Candidate")]
    [HttpGet("status/{jobPostingId}")]
    public async Task<IActionResult> GetPaymentStatus(long jobPostingId)
    {
        var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(keycloakUserId)) return Unauthorized();

        var candidate = await _unitOfWork.Context.Set<Candidate>()
            .FirstOrDefaultAsync(c => c.KeycloakUserId == keycloakUserId);
        if (candidate == null) return Unauthorized();

        var fee = await _unitOfWork.Context.Set<ApplicationFee>()
            .Where(f => f.JobPostingId == jobPostingId && f.IsActive)
            .Select(f => new { f.ApplicationFeeId, f.Amount, f.Currency })
            .FirstOrDefaultAsync();

        if (fee == null) return Ok(new { hasFee = false });

        var paid = await _unitOfWork.Context.Set<PaymentTransaction>()
            .AnyAsync(t => t.CandidateId == candidate.CandidateId
                && t.JobPostingId == jobPostingId
                && t.PaymentStatus == PaymentStatusEnum.Completed
                && t.IsActive);

        return Ok(new { hasFee = true, fee.Amount, fee.Currency, isPaid = paid });
    }

    [AllowAnonymous]
    [HttpPost("success")]
    [HttpGet("success")]
    public async Task<IActionResult> PaymentSuccess()
    {
        try
        {
        var tranId = Request.Method == "GET"
            ? (Request.Query["tran_id"].ToString() ?? "")
            : (Request.Form["tran_id"].ToString() ?? "");

        var transaction = await _unitOfWork.Context.Set<PaymentTransaction>()
            .FirstOrDefaultAsync(t => t.TransactionId == tranId && t.IsActive);

        if (transaction != null && transaction.PaymentStatus != PaymentStatusEnum.Completed)
        {
            transaction.PaymentStatus = PaymentStatusEnum.Completed;
            transaction.PaidAt = DateTime.UtcNow;
            transaction.BankTransactionId = Request.Method == "GET"
                ? Request.Query["bank_tran_id"].ToString()
                : Request.Form["bank_tran_id"].ToString();
            transaction.CardType = Request.Method == "GET"
                ? Request.Query["card_type"].ToString()
                : Request.Form["card_type"].ToString();

            var candidate = await _unitOfWork.Context.Set<Candidate>()
                .FirstOrDefaultAsync(c => c.CandidateId == transaction.CandidateId);
            var job = await _unitOfWork.Context.Set<JobPosting>()
                .FirstOrDefaultAsync(j => j.JobPostingId == transaction.JobPostingId);

            var existingApp = await _unitOfWork.Context.Set<JobApplication>()
                .FirstOrDefaultAsync(a => a.JobPostingId == transaction.JobPostingId
                    && a.CandidateId == transaction.CandidateId);

            var alreadyApplied = existingApp is { IsActive: true };

            if (!alreadyApplied && candidate != null && job != null)
            {
                if (existingApp != null)
                {
                    existingApp.IsActive = true;
                    existingApp.ApplicationStatus = ApplicationStatusEnum.Applied;
                    existingApp.AppliedDate = DateTime.UtcNow;
                    existingApp.CandidateName = candidate.FullName ?? "";
                    existingApp.CandidateEmail = candidate.Email;
                    existingApp.CandidatePhone = candidate.Phone;
                }
                else
                {
                    var application = new JobApplication
                    {
                        JobPostingId = transaction.JobPostingId,
                        CandidateId = transaction.CandidateId,
                        CandidateName = candidate.FullName ?? "",
                        CandidateEmail = candidate.Email,
                        CandidatePhone = candidate.Phone,
                        ApplicationStatus = ApplicationStatusEnum.Applied,
                        AppliedDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    await _unitOfWork.Context.Set<JobApplication>().AddAsync(application);
                }

                await _unitOfWork.Context.Set<UserNotification>().AddAsync(new UserNotification
                {
                    KeycloakUserId = candidate.KeycloakUserId ?? "",
                    Title = "Application Submitted",
                    Message = $"Your application for \"{job.Title}\" has been submitted after payment.",
                    NotificationType = UserNotificationTypeEnum.Success,
                    ActionUrl = "/my-applications",
                    IsActive = true
                });

                var hrUsers = await _unitOfWork.Context.Set<Domain.Entities.User>()
                    .Where(u => u.IsActive && u.KeycloakUserId != (candidate.KeycloakUserId ?? ""))
                    .Select(u => u.KeycloakUserId)
                    .ToListAsync();

                foreach (var hrKeycloakId in hrUsers)
                {
                    await _unitOfWork.Context.Set<UserNotification>().AddAsync(new UserNotification
                    {
                        KeycloakUserId = hrKeycloakId,
                        Title = "New Application Received",
                        Message = $"{candidate.FullName ?? "Candidate"} applied for \"{job.Title}\" position.",
                        NotificationType = UserNotificationTypeEnum.Info,
                        ActionUrl = $"/job-postings/{transaction.JobPostingId}/pipeline",
                        IsActive = true
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync();

            if (!alreadyApplied && candidate != null && job != null)
            {
                var candidateName = candidate.FullName ?? "Candidate";
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendAsync(
                            candidate.Email ?? "", candidateName,
                            $"Application Submitted — {job.Title}",
                            EmailTemplates.ApplicationSubmittedCandidate(candidateName, job.Title));

                        if (!string.IsNullOrEmpty(_emailSettings.HrNotificationEmail))
                        {
                            await _emailService.SendAsync(
                                _emailSettings.HrNotificationEmail, "HR Team",
                                $"New Application — {job.Title}",
                                EmailTemplates.ApplicationReceivedHR(candidateName, candidate.Email ?? "", job.Title));
                        }
                    }
                    catch { }
                });
            }
        }

        return Redirect($"http://localhost:4200/payment/result?status=success&tran_id={Uri.EscapeDataString(tranId)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PaymentSuccess failed");
            return Redirect($"http://localhost:4200/payment/result?status=success&error={Uri.EscapeDataString(ex.Message)}");
        }
    }

    [AllowAnonymous]
    [HttpPost("fail")]
    [HttpGet("fail")]
    public IActionResult PaymentFail()
    {
        var tranId = Request.Method == "GET"
            ? Request.Query["tran_id"].ToString()
            : Request.Form["tran_id"].ToString();
        return Redirect($"http://localhost:4200/payment/result?status=failed&tran_id={Uri.EscapeDataString(tranId)}");
    }

    [AllowAnonymous]
    [HttpPost("cancel")]
    [HttpGet("cancel")]
    public IActionResult PaymentCancel()
    {
        var tranId = Request.Method == "GET"
            ? Request.Query["tran_id"].ToString()
            : Request.Form["tran_id"].ToString();
        return Redirect($"http://localhost:4200/payment/result?status=cancelled&tran_id={Uri.EscapeDataString(tranId)}");
    }
}

public class InitiatePaymentRequest
{
    public long JobPostingId { get; set; }
}
