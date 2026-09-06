using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A configurable hiring workflow (ordered set of stages) that a job posting is bound to.
/// Reusable across many job postings so HR is not re-modeling the same pipeline per job.
/// </summary>
public class HiringPipeline : Audit, ICompanyScoped
{
    public long HiringPipelineId { get; set; }

    // Multi-tenant: the Company that owns this pipeline, stamped from the creating Admin/HR user's own company.
    public long? CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<PipelineStage> Stages { get; set; } = new List<PipelineStage>();
    public ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
}
