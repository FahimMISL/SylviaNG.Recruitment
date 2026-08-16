using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-12 US-097: HR-configured grade/designation/salary structure for one JobApplication, 1:1 (same
/// unique-FK shape as PreBoardingSubmission<->FinalSelectionPool). Manual entry only - no Payroll
/// auto-fetch (EP-16 System Integrations is out of scope for this project). Designation/Location are
/// plain strings, matching OfferLetter.Designation/JobPosting.Location - no local Designation lookup
/// table exists anywhere in this codebase. Salary is flat decimals (Basic/Allowances/Deductions), no
/// itemized line-item child entities - no precedent for that shape exists here either.
/// </summary>
public class FitmentData : Audit, ICompanyScoped
{
    public long FitmentDataId { get; set; }
    public long JobApplicationId { get; set; }

    // Multi-tenant: denormalized from JobApplication.CompanyId at creation time.
    public long? CompanyId { get; set; }

    public string Designation { get; set; } = string.Empty;
    public string? Grade { get; set; }
    public string? Location { get; set; }

    public decimal BasicSalary { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }

    public JobApplication JobApplication { get; set; } = null!;
}
