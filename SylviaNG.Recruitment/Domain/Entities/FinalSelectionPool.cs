using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-12 US-094: entry point of the onboarding data pipeline. One row per Accepted OfferLetter,
/// auto-created inside OfferLetterService.AcceptAsync (see FinalSelectionPoolService.
/// CreateFromAcceptedOfferAsync) - resolves the "no FinalSelectionPool entity exists yet" gap
/// JoiningBooklet.cs's own doc-comment flagged. JoiningDate seeds from OfferLetter.JoiningDate but
/// is HR-editable afterward (batches get reassigned), which JoiningBooklet's ad-hoc BatchLabel stub
/// cannot do.
/// </summary>
public class FinalSelectionPool : Audit
{
    public long FinalSelectionPoolId { get; set; }
    public long OfferLetterId { get; set; }
    public long JobApplicationId { get; set; }

    public string? BatchLabel { get; set; }
    public DateTime JoiningDate { get; set; }
    public bool HasJoined { get; set; }
    public DateTime? JoinedAt { get; set; }
    public DateTime EnteredPoolAt { get; set; }

    public OfferLetter OfferLetter { get; set; } = null!;
    public JobApplication JobApplication { get; set; } = null!;
    public PreBoardingSubmission? PreBoardingSubmission { get; set; }
}
