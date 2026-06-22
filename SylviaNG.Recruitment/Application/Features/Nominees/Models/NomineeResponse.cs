namespace SylviaNG.Recruitment.Application.Features.Nominees.Models
{
    public class NomineeResponse
    {
        public long NomineeId { get; set; }
        public long PreBoardingProfileId { get; set; }
        public string NomineeName { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? NationalIdNumber { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public decimal? SharePercentage { get; set; }
        public string? IdProofFileUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
