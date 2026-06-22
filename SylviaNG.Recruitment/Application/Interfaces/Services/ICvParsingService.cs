using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Interfaces.Services;

public interface ICvParsingService
{
    Task<CvParsedData> ParseCvAsync(Stream fileStream, string fileName);
}

public class CvParsedData
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? PresentAddress { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? CurrentDesignation { get; set; }
    public string? CurrentOrganization { get; set; }
    public int? TotalExperienceYears { get; set; }
    public List<ParsedEducation> Educations { get; set; } = new();
    public List<ParsedExperience> Experiences { get; set; } = new();
    public List<string> Skills { get; set; } = new();
    public decimal ConfidenceScore { get; set; }
}

public class ParsedEducation
{
    public string? Degree { get; set; }
    public string? Institution { get; set; }
    public string? FieldOfStudy { get; set; }
    public int? PassingYear { get; set; }
    public string? Result { get; set; }
}

public class ParsedExperience
{
    public string? CompanyName { get; set; }
    public string? Designation { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? Description { get; set; }
}
