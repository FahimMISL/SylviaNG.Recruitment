namespace SylviaNG.Recruitment.Application.Features.PreBoarding.Models
{
    public class PreBoardingNomineeResponse
    {
        public long PreBoardingNomineeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public decimal SharePercentage { get; set; }
        public string? ContactPhone { get; set; }
        public string? Address { get; set; }
    }
}
