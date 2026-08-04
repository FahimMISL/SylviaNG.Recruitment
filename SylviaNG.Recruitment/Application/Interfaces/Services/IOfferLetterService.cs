using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IOfferLetterService
    {
        Task<OfferLetterResponse> GenerateAsync(OfferLetterGenerateRequest request);
        Task<List<OfferLetterResponse>> GetAllAsync(long? jobApplicationId);
        Task<OfferLetterResponse> GetByIdAsync(long offerLetterId);

        // Warn-only: other applications from the same candidate already Hired elsewhere. See
        // CandidateHireConflictResponse's doc comment for why this exists and why it never blocks.
        Task<List<CandidateHireConflictResponse>> GetCandidateHireConflictsAsync(long jobApplicationId);

        // EP-10 US-082: candidate self-service - identity is resolved internally via
        // ICurrentCandidateService, not passed in by the caller (same convention as
        // ExamTakingService.GetOwnedEnrollmentAsync).
        Task<List<OfferLetterResponse>> GetAllForCandidateAsync();
        Task<OfferLetterResponse> GetByIdForCandidateAsync(long offerLetterId);
        Task<OfferLetterResponse> AcceptAsync(long offerLetterId);
        Task<OfferLetterResponse> DeclineAsync(long offerLetterId, string reason);
    }
}
