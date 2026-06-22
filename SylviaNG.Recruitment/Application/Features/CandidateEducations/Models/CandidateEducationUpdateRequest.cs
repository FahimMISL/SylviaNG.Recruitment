namespace SylviaNG.Recruitment.Application.Features.CandidateEducations.Models
{
    public class CandidateEducationUpdateRequest
    {
        public long? CandidateId { get; set; }
        public string? Degree { get; set; }
        public string? FieldOfStudy { get; set; }
        public string? Institution { get; set; }
        public string? Board { get; set; }
        public int? PassingYear { get; set; }
        public string? Result { get; set; }
        public string? ResultScale { get; set; }
        public bool? IsActive { get; set; }
    }
}
