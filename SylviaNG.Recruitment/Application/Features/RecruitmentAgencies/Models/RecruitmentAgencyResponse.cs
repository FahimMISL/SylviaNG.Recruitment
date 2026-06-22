namespace SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Models
{
    public class RecruitmentAgencyResponse
    {
        public long RecruitmentAgencyId { get; set; }
        public string AgencyName { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? AgreementDetails { get; set; }
        public DateTime? AgreementStartDate { get; set; }
        public DateTime? AgreementEndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
