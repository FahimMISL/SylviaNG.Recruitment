using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A named, reusable assessment workflow (ordered set of stages) that a job posting can be
/// bound to (US-051). Reusable across many job postings so HR is not re-modeling the same
/// workflow per job, same convention as HiringPipeline.
/// </summary>
public class AssessmentWorkflow : Audit
{
    public long AssessmentWorkflowId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<AssessmentStage> Stages { get; set; } = new List<AssessmentStage>();
    public ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
}
