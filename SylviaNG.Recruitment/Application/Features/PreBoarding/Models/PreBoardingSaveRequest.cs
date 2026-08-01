namespace SylviaNG.Recruitment.Application.Features.PreBoarding.Models
{
    public class PreBoardingSaveRequest
    {
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactRelationship { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;

        public string? InsuranceProvider { get; set; }
        public string? InsurancePolicyNumber { get; set; }
        public string? InsuranceNotes { get; set; }

        public string BankName { get; set; } = string.Empty;
        public string? BankBranch { get; set; }
        public string BankAccountName { get; set; } = string.Empty;
        public string BankAccountNumber { get; set; } = string.Empty;
        public string? BankRoutingNumber { get; set; }

        public List<PreBoardingNomineeRequest> Nominees { get; set; } = new();
    }
}
