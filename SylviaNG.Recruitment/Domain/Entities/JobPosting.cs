using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class JobPosting : Audit
{
    public long JobPostingId { get; set; }
    public long SiteId { get; set; }
    public long? DepartmentId { get; set; }
    public long? DesignationId { get; set; }
    public long? RequisitionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Requirements { get; set; }
    public int NumberOfPositions { get; set; } = 1;
    public EmploymentTypeEnum EmploymentType { get; set; } = EmploymentTypeEnum.FullTime;
    public new JobStatusEnum Status { get; set; } = JobStatusEnum.Draft;
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public DateTime? PostingDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Eligibility filters
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public int? MinExperienceYears { get; set; }
    public EducationLevelEnum? MinEducationLevel { get; set; }
    public string? RequiredDistrict { get; set; }

    // Navigation properties
    public Requisition? Requisition { get; set; }
    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    public ICollection<JobPostingChannel> Channels { get; set; } = new List<JobPostingChannel>();
    public ICollection<HiringPipelineStage> HiringPipelineStages { get; set; } = new List<HiringPipelineStage>();
}
