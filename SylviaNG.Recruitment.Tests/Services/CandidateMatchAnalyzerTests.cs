using FluentAssertions;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Tests.Services;

public class CandidateMatchAnalyzerTests
{
    private static CandidateFactService.CandidateFacts Facts(double experience = 0, HashSet<string>? skills = null) =>
        new(null, experience, skills ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase), new HashSet<EducationLevelEnum>(), string.Empty, new HashSet<string>());

    [Fact]
    public void GetMatchedSkills_SkillsFoundInRequirements_ShouldReturnOnlyMatched()
    {
        var posting = new JobPosting { Requirements = "Looking for C#, SQL experts", Description = "Backend role" };
        var facts = Facts(skills: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "C#", "SQL", "Python" });

        var matched = CandidateMatchAnalyzer.GetMatchedSkills(posting, facts);

        matched.Should().BeEquivalentTo(new[] { "C#", "SQL" });
    }

    [Fact]
    public void GetMatchedSkills_NoRequirementsText_ShouldReturnEmpty()
    {
        var posting = new JobPosting();
        var facts = Facts(skills: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "C#" });

        var matched = CandidateMatchAnalyzer.GetMatchedSkills(posting, facts);

        matched.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0.5, "0-1 years")]
    [InlineData(2, "1-3 years")]
    [InlineData(4, "3-5 years")]
    [InlineData(7, "5-10 years")]
    [InlineData(12, "10+ years")]
    public void GetExperienceBand_ShouldBucketCorrectly(double years, string expectedBand)
    {
        CandidateMatchAnalyzer.GetExperienceBand(years).Should().Be(expectedBand);
    }
}
