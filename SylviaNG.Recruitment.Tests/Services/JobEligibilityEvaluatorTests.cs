using FluentAssertions;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Tests.Services;

public class JobEligibilityEvaluatorTests
{
    private static CandidateFactService.CandidateFacts Facts(
        int? age = null,
        double experienceYears = 0,
        IEnumerable<EducationLevelEnum>? educationLevels = null,
        string address = "") =>
        new(age, experienceYears,
            new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            new HashSet<EducationLevelEnum>(educationLevels ?? Enumerable.Empty<EducationLevelEnum>()),
            address);

    private static JobPosting Posting(
        int? minAge = null,
        int? maxAge = null,
        EducationLevelEnum? minEducationLevel = null,
        int? minExperienceYears = null,
        string? requiredDistrict = null) =>
        new()
        {
            JobPostingId = 1,
            Title = "Software Engineer",
            MinAge = minAge,
            MaxAge = maxAge,
            MinEducationLevel = minEducationLevel,
            MinExperienceYears = minExperienceYears,
            RequiredDistrict = requiredDistrict
        };

    [Fact]
    public void Evaluate_NoRequirements_ShouldReturnEmpty()
    {
        var result = JobEligibilityEvaluator.Evaluate(Posting(), Facts(age: 25));

        result.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_MinAge_UnknownAge_ShouldBeUnmet()
    {
        var result = JobEligibilityEvaluator.Evaluate(Posting(minAge: 25), Facts(age: null));

        result.Should().ContainSingle().Which.Should().Contain("25");
    }

    [Fact]
    public void Evaluate_MinAge_BelowMinimum_ShouldBeUnmet()
    {
        var result = JobEligibilityEvaluator.Evaluate(Posting(minAge: 25), Facts(age: 20));

        result.Should().ContainSingle();
    }

    [Fact]
    public void Evaluate_MinAge_AtOrAboveMinimum_ShouldBeMet()
    {
        var result = JobEligibilityEvaluator.Evaluate(Posting(minAge: 25), Facts(age: 25));

        result.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_MaxAge_AboveMaximum_ShouldBeUnmet()
    {
        var result = JobEligibilityEvaluator.Evaluate(Posting(maxAge: 35), Facts(age: 40));

        result.Should().ContainSingle();
    }

    [Fact]
    public void Evaluate_MinEducationLevel_NoDegreeMeetingMinimum_ShouldBeUnmet()
    {
        var result = JobEligibilityEvaluator.Evaluate(
            Posting(minEducationLevel: EducationLevelEnum.Bachelor),
            Facts(educationLevels: new[] { EducationLevelEnum.SSC }));

        result.Should().ContainSingle();
    }

    [Fact]
    public void Evaluate_MinEducationLevel_AnyDegreeMeetingMinimum_ShouldBeMet()
    {
        var result = JobEligibilityEvaluator.Evaluate(
            Posting(minEducationLevel: EducationLevelEnum.Diploma),
            Facts(educationLevels: new[] { EducationLevelEnum.SSC, EducationLevelEnum.Bachelor }));

        result.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_MinExperienceYears_BelowMinimum_ShouldBeUnmet()
    {
        var result = JobEligibilityEvaluator.Evaluate(
            Posting(minExperienceYears: 3),
            Facts(experienceYears: 1.5));

        result.Should().ContainSingle();
    }

    [Fact]
    public void Evaluate_RequiredDistrict_NotInAddress_ShouldBeUnmet()
    {
        var result = JobEligibilityEvaluator.Evaluate(
            Posting(requiredDistrict: "Dhaka"),
            Facts(address: "House 12, Road 5, Chattogram"));

        result.Should().ContainSingle().Which.Should().Contain("Dhaka");
    }

    [Fact]
    public void Evaluate_RequiredDistrict_CaseInsensitiveSubstringInAddress_ShouldBeMet()
    {
        var result = JobEligibilityEvaluator.Evaluate(
            Posting(requiredDistrict: "dhanmondi"),
            Facts(address: "House 12, Road 5, Dhanmondi, Dhaka"));

        result.Should().BeEmpty();
    }

    [Fact]
    public void Evaluate_CombinedRequirements_AllUnmetShouldBeReported()
    {
        var posting = Posting(minAge: 25, maxAge: 35, minEducationLevel: EducationLevelEnum.Bachelor,
            minExperienceYears: 3, requiredDistrict: "Dhaka");
        var facts = Facts(age: 20, experienceYears: 1, educationLevels: new[] { EducationLevelEnum.SSC }, address: "Chattogram");

        var result = JobEligibilityEvaluator.Evaluate(posting, facts);

        result.Should().HaveCount(4);
    }
}
