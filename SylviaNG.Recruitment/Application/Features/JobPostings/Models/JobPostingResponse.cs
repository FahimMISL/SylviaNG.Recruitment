using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    public class JobPostingResponse
    {
        public long JobPostingId { get; set; }
        public string JobPostingCode { get; set; } = string.Empty;
        public long SiteId { get; set; }
        public string? SiteName { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public long? DesignationId { get; set; }
        public string? DesignationName { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Requirements { get; set; }
        public int NumberOfPositions { get; set; }
        public EmploymentTypeEnum EmploymentType { get; set; }
        public JobStatusEnum Status { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public DateTime? PostingDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public bool IsActive { get; set; }
        public int TotalApplications { get; set; }

        // EP-02: Job Vacancy Configuration fields
        public string? Location { get; set; }
        public CircularTypeEnum CircularType { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public EducationLevelEnum? MinEducationLevel { get; set; }
        public int? MinExperienceYears { get; set; }
        public string? RequiredDistrict { get; set; }
        public decimal? ApplicationFeeAmount { get; set; }
        public string? ApplicationFeeCurrency { get; set; }
        public long? HiringPipelineId { get; set; }
        public string? HiringPipelineName { get; set; }
    }
}
