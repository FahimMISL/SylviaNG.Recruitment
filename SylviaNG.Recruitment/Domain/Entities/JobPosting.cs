using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Represents a job posting/requisition in the recruitment system.
/// </summary>
public class JobPosting : Audit
{
    public long JobPostingId { get; set; }
    public long SiteId { get; set; }
    public long? DepartmentId { get; set; }
    public long? DesignationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Requirements { get; set; }
    public int NumberOfPositions { get; set; } = 1;
    public EmploymentTypeEnum EmploymentType { get; set; } = EmploymentTypeEnum.FullTime;
    public new JobStatusEnum Status { get; set; } = JobStatusEnum.Draft;
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public DateTime? PostingDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public bool IsActive { get; set; } = true;

    // EP-02: Job Vacancy Configuration fields
    public string JobPostingCode { get; set; } = string.Empty;
    public string? Location { get; set; }
    public CircularTypeEnum CircularType { get; set; } = CircularTypeEnum.Both;
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public EducationLevelEnum? MinEducationLevel { get; set; }
    public int? MinExperienceYears { get; set; }
    public string? RequiredDistrict { get; set; }
    public decimal? ApplicationFeeAmount { get; set; }
    public string? ApplicationFeeCurrency { get; set; }

    // Hiring pipeline binding. Nullable at the DB level so pre-existing job postings created
    // before this feature aren't broken; JobPostingCreateValidator makes it mandatory for new
    // job postings per the "every job opening must reference a pipeline" requirement.
    public long? HiringPipelineId { get; set; }

    // Assessment workflow binding (US-051). Optional - unlike HiringPipelineId this is not
    // mandatory for new job postings, just an available link.
    public long? AssessmentWorkflowId { get; set; }

    // Navigation properties
    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    public ICollection<JobPostingAttachment> Attachments { get; set; } = new List<JobPostingAttachment>();
    public HiringPipeline? HiringPipeline { get; set; }
    public AssessmentWorkflow? AssessmentWorkflow { get; set; }
}
