namespace SylviaNG.Recruitment.Application.Features.CandidateEducations.Models
{
    public class CandidateEducationResponse
    {
        public long CandidateEducationId { get; set; }
        public long CandidateId { get; set; }
        public string Degree { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public string? Institution { get; set; }
        public string? Board { get; set; }
        public int? PassingYear { get; set; }
        public string? Result { get; set; }
        public string? ResultScale { get; set; }
        public bool IsActive { get; set; }
    }
}
