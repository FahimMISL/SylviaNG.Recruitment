using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A single SSLCommerz payment attempt against a JobApplication (EP-17). One-to-many with
/// JobApplication - a candidate may retry after a failed/cancelled attempt, and each attempt
/// keeps its own audit trail (TransactionId/ValidationId/RawIpnPayload).
/// </summary>
public class Payment : Audit
{
    public long PaymentId { get; set; }
    public long JobApplicationId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;

    /// <summary>Our own generated tran_id sent to SSLCommerz on session init. Unique per attempt.</summary>
    public string TransactionId { get; set; } = string.Empty;

    public string? GatewaySessionKey { get; set; }
    public string? GatewayRedirectUrl { get; set; }
    public PaymentStatusEnum PaymentStatus { get; set; } = PaymentStatusEnum.Pending;

    /// <summary>SSLCommerz's val_id, present once the candidate completes checkout; used to call
    /// their Validation API - the only source of truth for whether a payment actually succeeded.</summary>
    public string? ValidationId { get; set; }

    /// <summary>Raw IPN POST body, kept for audit/debugging. Never trusted on its own for status.</summary>
    public string? RawIpnPayload { get; set; }

    public DateTime? PaidAt { get; set; }

    // Navigation properties
    public JobApplication JobApplication { get; set; } = null!;
}
