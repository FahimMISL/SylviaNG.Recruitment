using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// The application status state machine. Stateless and pure, so it stays a plain static class
    /// (no interface, no DI) rather than a service - shared directly by JobApplicationStatusService
    /// and JobApplicationSelfService, which both need to validate/enforce the same legal
    /// transitions (a status service driving an HR-initiated move, a self-service candidate
    /// withdrawal) without duplicating the rule table between them.
    /// </summary>
    internal static class JobApplicationStatusRules
    {
        // US-036 AC1: legal application status transitions. Reject/withdraw is reachable from any
        // active stage (not just the end of the pipeline); Hired/Rejected/Withdrawn are terminal.
        // Applied->Shortlisted is legal directly (not just via Screening) so US-044's automated
        // filter apply can fast-track a fresh application without a manual screening bump first -
        // that's the whole point of "without manual screening of every application" in US-043.
        // US-038 AC3/AC4: DuplicateDismissed is reachable from any active stage, same as Withdrawn -
        // HR can resolve a duplicate at whatever point in the pipeline it's noticed.
        internal static readonly Dictionary<ApplicationStatusEnum, ApplicationStatusEnum[]> LegalStatusTransitions = new()
        {
            [ApplicationStatusEnum.Applied] = new[] { ApplicationStatusEnum.Screening, ApplicationStatusEnum.Shortlisted, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn, ApplicationStatusEnum.DuplicateDismissed },
            [ApplicationStatusEnum.Screening] = new[] { ApplicationStatusEnum.Shortlisted, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn, ApplicationStatusEnum.DuplicateDismissed },
            [ApplicationStatusEnum.Shortlisted] = new[] { ApplicationStatusEnum.InterviewScheduled, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn, ApplicationStatusEnum.DuplicateDismissed },
            [ApplicationStatusEnum.InterviewScheduled] = new[] { ApplicationStatusEnum.Interviewed, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn, ApplicationStatusEnum.DuplicateDismissed },
            [ApplicationStatusEnum.Interviewed] = new[] { ApplicationStatusEnum.Offered, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn, ApplicationStatusEnum.DuplicateDismissed },
            [ApplicationStatusEnum.Offered] = new[] { ApplicationStatusEnum.Hired, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn, ApplicationStatusEnum.DuplicateDismissed },
            [ApplicationStatusEnum.Hired] = Array.Empty<ApplicationStatusEnum>(),
            [ApplicationStatusEnum.Rejected] = Array.Empty<ApplicationStatusEnum>(),
            [ApplicationStatusEnum.Withdrawn] = Array.Empty<ApplicationStatusEnum>(),

            // EP-17: Applied = payment confirmed via SSLCommerz IPN (PaymentService writes this
            // transition directly, bypassing this HR-user-scoped path - see PaymentService.HandleIpnAsync).
            // Rejected lets HR close out a stale/abandoned unpaid application; Withdrawn lets a
            // candidate with an account cancel one they never intend to pay.
            [ApplicationStatusEnum.AwaitingPayment] = new[] { ApplicationStatusEnum.Applied, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn },
            [ApplicationStatusEnum.DuplicateDismissed] = Array.Empty<ApplicationStatusEnum>()
        };

        internal static void EnsureLegalStatusTransition(ApplicationStatusEnum currentStatus, ApplicationStatusEnum requestedStatus)
        {
            if (!LegalStatusTransitions.TryGetValue(currentStatus, out var allowedTransitions)
                || !allowedTransitions.Contains(requestedStatus))
            {
                throw new InvalidStatusTransitionException("JobApplication", currentStatus, requestedStatus);
            }
        }
    }
}
