using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    /// <summary>
    /// Best-effort result of parsing an uploaded resume, produced by either the "Heuristic"
    /// (free, local, regex-based) or "Ai" (Groq-backed) IResumeParsingService implementation -
    /// see ParsingProvider/AiParsingDegraded. Frontend prefills form controls from this; the
    /// candidate must still review and hit Save per section for the extracted fields. The
    /// uploaded file itself is saved separately as a Resume-type CandidateDocument in the same
    /// request (see ResumeDocumentSaved) so the candidate never has to upload it a second time.
    /// </summary>
    public class CandidateResumeParseResponse
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? PresentAddress { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Best-effort guessed text (e.g. "Male", "Islam", "Single") matching the seeded
        // Gender/Religion/MaritalStatus lookup Name values - these are now dynamic admin-managed
        // tables, so the parser can't resolve a specific row Id; frontend matches by name.
        public string? Gender { get; set; }
        public string? Religion { get; set; }
        public string? MaritalStatus { get; set; }
        public List<string> Skills { get; set; } = new();
        public List<CandidateResumeParsedEducation> Educations { get; set; } = new();
        public List<CandidateResumeParsedWorkExperience> WorkExperiences { get; set; } = new();

        /// <summary>Which provider actually produced this result ("Heuristic" or "Ai"), after any fallback.</summary>
        public string ParsingProvider { get; set; } = "Heuristic";

        /// <summary>True only when the Ai provider was configured/attempted but fell back to Heuristic.</summary>
        public bool AiParsingDegraded { get; set; }

        /// <summary>
        /// True when the uploaded file was also saved as a Resume document (visible/downloadable
        /// from the Documents section). False on the rare case that save failed - parsing itself
        /// still succeeds and returns above, so a document-save failure never blocks prefill.
        /// </summary>
        public bool ResumeDocumentSaved { get; set; }
    }

    public class CandidateResumeParsedEducation
    {
        public string? DegreeTitle { get; set; }
        public string? Institution { get; set; }
        public EducationLevelEnum? EducationLevel { get; set; }
        public long? UniversityLibraryItemId { get; set; }
        public int? PassingYear { get; set; }
        public string? Result { get; set; }
        public string? MajorSubject { get; set; }
    }

    public class CandidateResumeParsedWorkExperience
    {
        public string? CompanyName { get; set; }
        public string? Designation { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string? Responsibilities { get; set; }
        public string? Location { get; set; }
    }
}
