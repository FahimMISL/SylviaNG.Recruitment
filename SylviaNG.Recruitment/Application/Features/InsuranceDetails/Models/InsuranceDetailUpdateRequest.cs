namespace SylviaNG.Recruitment.Application.Features.InsuranceDetails.Models
{
    public class InsuranceDetailUpdateRequest
    {
        public long? PreBoardingProfileId { get; set; }
        public string? InsuranceType { get; set; }
        public string? ProviderName { get; set; }
        public string? PolicyNumber { get; set; }
        public string? BeneficiaryName { get; set; }
        public string? BeneficiaryRelationship { get; set; }
        public string? DocumentFileUrl { get; set; }
        public bool? IsActive { get; set; }
    }
}
