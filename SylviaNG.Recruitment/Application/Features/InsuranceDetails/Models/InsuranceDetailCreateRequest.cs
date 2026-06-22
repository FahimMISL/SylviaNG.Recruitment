namespace SylviaNG.Recruitment.Application.Features.InsuranceDetails.Models
{
    public class InsuranceDetailCreateRequest
    {
        public long PreBoardingProfileId { get; set; }
        public string InsuranceType { get; set; } = string.Empty;
        public string? ProviderName { get; set; }
        public string? PolicyNumber { get; set; }
        public string? BeneficiaryName { get; set; }
        public string? BeneficiaryRelationship { get; set; }
        public string? DocumentFileUrl { get; set; }
    }
}
