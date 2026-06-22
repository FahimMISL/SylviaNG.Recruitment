using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.MedicalTests.Models
{
    public class MedicalTestResponse
    {
        public long MedicalTestId { get; set; }
        public long VerificationWorkflowId { get; set; }
        public MedicalFitnessEnum FitnessStatus { get; set; }
        public string? MedicalCenter { get; set; }
        public DateTime? TestDate { get; set; }
        public string? ResultFileUrl { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
    }
}
