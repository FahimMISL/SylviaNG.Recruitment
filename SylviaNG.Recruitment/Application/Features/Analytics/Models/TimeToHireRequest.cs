namespace SylviaNG.Recruitment.Application.Features.Analytics.Models
{
    /// <summary>EP-14 US-107 AC4: JobPostingId/DepartmentId (raw external ID)/DateRange filter
    /// scope by vacancy (DateFrom/DateTo filter on JobPosting.PostingDate). No HiringManager
    /// field exists anywhere in the schema, so that AC4 filter is dropped - see feature doc.</summary>
    public class TimeToHireRequest
    {
        public long? JobPostingId { get; set; }
        public long? DepartmentId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
