using FluentAssertions;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Tests.Services;

public class ManualShortlistScoringServiceTests
{
    private readonly ManualShortlistScoringService _service = new();

    private static JobPosting Posting(
        EducationLevelEnum? minEducation = null,
        int? minExperience = null,
        string? requirements = null,
        string? description = null,
        string? requiredDistrict = null,
        int? minAge = null,
        int? maxAge = null) =>
        new()
        {
            Title = "Software Engineer",
            MinEducationLevel = minEducation,
            MinExperienceYears = minExperience,
            Requirements = requirements,
            Description = description,
            RequiredDistrict = requiredDistrict,
            MinAge = minAge,
            MaxAge = maxAge
        };

    private static CandidateFactService.CandidateFacts Facts(
        int? age = null,
        double experience = 0,
        HashSet<string>? skills = null,
        HashSet<EducationLevelEnum>? educationLevels = null,
        string address = "") =>
        new(age, experience, skills ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase), educationLevels ?? new HashSet<EducationLevelEnum>(), address, new HashSet<string>());

    [Fact]
    public async Task ScoreAsync_NoRequirementsSet_ShouldScoreNeutralMax()
    {
        var result = await _service.ScoreAsync(Posting(), Facts());

        result.Failed.Should().BeFalse();
        result.Score.Should().Be(100);
    }

    [Fact]
    public async Task ScoreAsync_MeetsEducationRequirement_ShouldAwardFullEducationPoints()
    {
        var posting = Posting(minEducation: EducationLevelEnum.Bachelor);
        var facts = Facts(educationLevels: new HashSet<EducationLevelEnum> { EducationLevelEnum.Master });

        var result = await _service.ScoreAsync(posting, facts);

        result.Score.Should().Be(100);
        result.Explanation.Should().Contain("Meets education requirement");
    }

    [Fact]
    public async Task ScoreAsync_BelowEducationRequirement_ShouldScoreZeroForEducation()
    {
        var posting = Posting(minEducation: EducationLevelEnum.Bachelor);
        var facts = Facts(educationLevels: new HashSet<EducationLevelEnum> { EducationLevelEnum.SSC });

        var result = await _service.ScoreAsync(posting, facts);

        result.Score.Should().Be(75); // 100 - 25 education points
        result.Explanation.Should().Contain("Does not meet education requirement");
    }

    [Fact]
    public async Task ScoreAsync_PartialExperience_ShouldAwardProportionalPoints()
    {
        var posting = Posting(minExperience: 4);
        var facts = Facts(experience: 2); // half of required -> 12/25 rounded

        var result = await _service.ScoreAsync(posting, facts);

        result.Score.Should().Be(87); // 100 - 25 + round(25*0.5)=13 => 75+13=88? computed below
    }

    [Fact]
    public async Task ScoreAsync_SkillsMatch_ShouldCapAtMaxPoints()
    {
        var posting = Posting(requirements: "Looking for React, SQL, Docker, Git, Java, Python experts");
        var facts = Facts(skills: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "React", "SQL", "Docker", "Git", "Java", "Python" });

        var result = await _service.ScoreAsync(posting, facts);

        // 6 matched skills * 6 pts = 36, capped at 30; other 4 components unset -> neutral max.
        // 25 (education) + 25 (experience) + 30 (skills, capped) + 10 (location) + 10 (age) = 100.
        result.Score.Should().Be(100);
        result.Explanation.Should().Contain("6/6 candidate skills matched");
    }

    [Fact]
    public async Task ScoreAsync_LocationMismatch_ShouldScoreZeroForLocation()
    {
        var posting = Posting(requiredDistrict: "Dhaka");
        var facts = Facts(address: "Chattogram");

        var result = await _service.ScoreAsync(posting, facts);

        result.Score.Should().Be(90); // 100 - 10 location points
        result.Explanation.Should().Contain("does not match required district");
    }

    [Fact]
    public async Task ScoreAsync_UnknownAgeWithAgeRangeSet_ShouldScoreZeroForAge()
    {
        var posting = Posting(minAge: 20, maxAge: 40);
        var facts = Facts(age: null);

        var result = await _service.ScoreAsync(posting, facts);

        result.Score.Should().Be(90); // 100 - 10 age points
        result.Explanation.Should().Contain("Age not on file");
    }

    [Fact]
    public async Task ScoreAsync_FullMatchAcrossAllComponents_ShouldScore100()
    {
        var posting = Posting(
            minEducation: EducationLevelEnum.Bachelor,
            minExperience: 3,
            requirements: "C#, SQL, Docker, Git, Azure",
            requiredDistrict: "Dhaka",
            minAge: 20,
            maxAge: 40);

        // 5 matched skills * 6 pts = 30 (the cap) - full marks needs >= 5 matches, not just "every listed skill".
        var facts = Facts(
            age: 28,
            experience: 5,
            skills: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "C#", "SQL", "Docker", "Git", "Azure" },
            educationLevels: new HashSet<EducationLevelEnum> { EducationLevelEnum.Bachelor },
            address: "Gulshan, Dhaka");

        var result = await _service.ScoreAsync(posting, facts);

        result.Score.Should().Be(100);
        result.Failed.Should().BeFalse();
    }
}
