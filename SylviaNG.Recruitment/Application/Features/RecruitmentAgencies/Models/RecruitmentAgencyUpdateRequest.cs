namespace SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Models
{
    public class RecruitmentAgencyUpdateRequest
    {
        public string? AgencyName { get; set; }
        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? AgreementDetails { get; set; }
        public DateTime? AgreementStartDate { get; set; }
        public DateTime? AgreementEndDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
