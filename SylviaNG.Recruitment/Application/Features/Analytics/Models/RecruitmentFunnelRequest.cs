namespace SylviaNG.Recruitment.Application.Features.Analytics.Models
{
    /// <summary>EP-14 US-106 AC2: JobPostingId/DepartmentId (raw external ID, no local name
    /// resolution) narrow scope; DateFrom/DateTo filter on JobApplication.AppliedDate; IsInternal
    /// is the "Candidate Type" filter (CandidateProfile.IsInternal), null = both.</summary>
    public class RecruitmentFunnelRequest
    {
        public long? JobPostingId { get; set; }
        public long? DepartmentId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool? IsInternal { get; set; }
    }
}
