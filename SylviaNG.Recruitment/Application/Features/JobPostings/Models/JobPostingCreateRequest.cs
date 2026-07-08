using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    public class JobPostingCreateRequest
    {
        public long SiteId { get; set; }
        public long? DepartmentId { get; set; }
        public long? DesignationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Requirements { get; set; }
        public int NumberOfPositions { get; set; } = 1;
        public EmploymentTypeEnum EmploymentType { get; set; } = EmploymentTypeEnum.FullTime;
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public DateTime? PostingDate { get; set; }
        public DateTime? ClosingDate { get; set; }

        // EP-02: Job Vacancy Configuration fields
        // Note: JobPostingCode is server-generated (never supplied by the client).
        public string? Location { get; set; }
        public CircularTypeEnum CircularType { get; set; } = CircularTypeEnum.Both;
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public EducationLevelEnum? MinEducationLevel { get; set; }
        public int? MinExperienceYears { get; set; }
        public string? RequiredDistrict { get; set; }
        public decimal? ApplicationFeeAmount { get; set; }
        public string? ApplicationFeeCurrency { get; set; }
    }
}
