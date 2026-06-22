namespace SylviaNG.Recruitment.Application.Features.Nominees.Models
{
    public class NomineeUpdateRequest
    {
        public long? PreBoardingProfileId { get; set; }
        public string? NomineeName { get; set; }
        public string? Relationship { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? NationalIdNumber { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public decimal? SharePercentage { get; set; }
        public string? IdProofFileUrl { get; set; }
        public bool? IsActive { get; set; }
    }
}
