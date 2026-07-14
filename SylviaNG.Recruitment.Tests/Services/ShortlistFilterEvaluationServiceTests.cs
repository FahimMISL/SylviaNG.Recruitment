using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Tests.Services;

public class ShortlistFilterEvaluationServiceTests
{
    private readonly Mock<IShortlistFilterRepository> _shortlistFilterRepositoryMock;
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<ICandidateProfileRepository> _candidateProfileRepositoryMock;
    private readonly ShortlistFilterEvaluationService _service;

    public ShortlistFilterEvaluationServiceTests()
    {
        _shortlistFilterRepositoryMock = new Mock<IShortlistFilterRepository>();
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _candidateProfileRepositoryMock = new Mock<ICandidateProfileRepository>();

        _service = new ShortlistFilterEvaluationService(
            _shortlistFilterRepositoryMock.Object,
            _jobApplicationRepositoryMock.Object,
            _candidateProfileRepositoryMock.Object);
    }

    private static JobApplication Application(long id, string email) =>
        new() { JobApplicationId = id, JobPostingId = 1, CandidateEmail = email };

    private static CandidateProfile Profile(
        string email,
        DateTime? dob = null,
        string? presentAddress = null,
        List<CandidateEducation>? educations = null,
        List<CandidateWorkExperience>? workExperiences = null,
        List<CandidateSkill>? skills = null) =>
        new()
        {
            Email = email,
            DateOfBirth = dob,
            PresentAddress = presentAddress,
            Educations = educations ?? new List<CandidateEducation>(),
            WorkExperiences = workExperiences ?? new List<CandidateWorkExperience>(),
            Skills = skills ?? new List<CandidateSkill>()
        };

    private void SetupApplicationsAndProfiles(List<JobApplication> applications, List<CandidateProfile> profiles)
    {
        _jobApplicationRepositoryMock.Setup(r => r.GetAllByJobPostingIdAsync(1)).ReturnsAsync(applications);
        _candidateProfileRepositoryMock
            .Setup(r => r.GetByEmailsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(profiles);
    }

    private static ShortlistFilterPreviewRequest DefinitionRequest(FilterCombinatorEnum combineWith, params ShortlistFilterCriterionRequest[] criteria) =>
        new()
        {
            JobPostingId = 1,
            Definition = new ShortlistFilterDefinitionRequest { CombineWith = combineWith, Criteria = criteria.ToList() }
        };

    [Fact]
    public async Task PreviewAsync_EducationLevelCriterion_AnyDegreeMeetingMinimum_ShouldPass()
    {
        // Arrange: candidate has SSC and Bachelor - Bachelor satisfies a Diploma minimum.
        var profile = Profile("a@x.com", educations: new List<CandidateEducation>
        {
            new() { EducationLevel = EducationLevelEnum.SSC },
            new() { EducationLevel = EducationLevelEnum.Bachelor }
        });
        SetupApplicationsAndProfiles(new List<JobApplication> { Application(1, "a@x.com") }, new List<CandidateProfile> { profile });

        var request = DefinitionRequest(FilterCombinatorEnum.And,
            new ShortlistFilterCriterionRequest { CriterionType = CriterionTypeEnum.EducationLevel, MinEducationLevel = EducationLevelEnum.Diploma });

        // Act
        var result = await _service.PreviewAsync(request);

        // Assert
        result.PassingCount.Should().Be(1);
    }

    [Fact]
    public async Task PreviewAsync_EducationLevelCriterion_BelowMinimum_ShouldFail()
    {
        var profile = Profile("a@x.com", educations: new List<CandidateEducation> { new() { EducationLevel = EducationLevelEnum.SSC } });
        SetupApplicationsAndProfiles(new List<JobApplication> { Application(1, "a@x.com") }, new List<CandidateProfile> { profile });

        var request = DefinitionRequest(FilterCombinatorEnum.And,
            new ShortlistFilterCriterionRequest { CriterionType = CriterionTypeEnum.EducationLevel, MinEducationLevel = EducationLevelEnum.Bachelor });

        var result = await _service.PreviewAsync(request);

        result.PassingCount.Should().Be(0);
    }

    [Fact]
    public async Task PreviewAsync_MinExperienceYearsCriterion_SumsAcrossRolesIncludingOngoing()
    {
        // Arrange: a completed 2-year role + an ongoing role that started 4 years ago (EndDate null) = ~6 years.
        var now = DateTime.UtcNow;
        var profile = Profile("a@x.com", workExperiences: new List<CandidateWorkExperience>
        {
            new() { StartDate = now.AddYears(-6), EndDate = now.AddYears(-4) },
            new() { StartDate = now.AddYears(-4), EndDate = null, IsCurrent = true }
        });
        SetupApplicationsAndProfiles(new List<JobApplication> { Application(1, "a@x.com") }, new List<CandidateProfile> { profile });

        var request = DefinitionRequest(FilterCombinatorEnum.And,
            new ShortlistFilterCriterionRequest { CriterionType = CriterionTypeEnum.MinExperienceYears, MinExperienceYears = 5 });

        var result = await _service.PreviewAsync(request);

        result.PassingCount.Should().Be(1);
    }

    [Fact]
    public async Task PreviewAsync_AgeRangeCriterion_OnlyMinSet_CandidateBelowMin_ShouldFail()
    {
        var profile = Profile("a@x.com", dob: DateTime.UtcNow.AddYears(-20));
        SetupApplicationsAndProfiles(new List<JobApplication> { Application(1, "a@x.com") }, new List<CandidateProfile> { profile });

        var request = DefinitionRequest(FilterCombinatorEnum.And,
            new ShortlistFilterCriterionRequest { CriterionType = CriterionTypeEnum.AgeRange, MinAge = 25 });

        var result = await _service.PreviewAsync(request);

        result.PassingCount.Should().Be(0);
    }

    [Fact]
    public async Task PreviewAsync_AgeRangeCriterion_NoDateOfBirth_ShouldFail()
    {
        var profile = Profile("a@x.com", dob: null);
        SetupApplicationsAndProfiles(new List<JobApplication> { Application(1, "a@x.com") }, new List<CandidateProfile> { profile });

        var request = DefinitionRequest(FilterCombinatorEnum.And,
            new ShortlistFilterCriterionRequest { CriterionType = CriterionTypeEnum.AgeRange, MinAge = 18, MaxAge = 60 });

        var result = await _service.PreviewAsync(request);

        result.PassingCount.Should().Be(0);
    }

    [Fact]
    public async Task PreviewAsync_RequiredSkillsCriterion_MissingOneOfSeveral_ShouldFail()
    {
        var profile = Profile("a@x.com", skills: new List<CandidateSkill> { new() { SkillName = "C#" } });
        SetupApplicationsAndProfiles(new List<JobApplication> { Application(1, "a@x.com") }, new List<CandidateProfile> { profile });

        var request = DefinitionRequest(FilterCombinatorEnum.And,
            new ShortlistFilterCriterionRequest { CriterionType = CriterionTypeEnum.RequiredSkills, RequiredSkillNames = "C#, SQL" });

        var result = await _service.PreviewAsync(request);

        result.PassingCount.Should().Be(0);
    }

    [Fact]
    public async Task PreviewAsync_RequiredSkillsCriterion_HasAllRequiredSkillsCaseInsensitive_ShouldPass()
    {
        var profile = Profile("a@x.com", skills: new List<CandidateSkill> { new() { SkillName = "c#" }, new() { SkillName = "sql" } });
        SetupApplicationsAndProfiles(new List<JobApplication> { Application(1, "a@x.com") }, new List<CandidateProfile> { profile });

        var request = DefinitionRequest(FilterCombinatorEnum.And,
            new ShortlistFilterCriterionRequest { CriterionType = CriterionTypeEnum.RequiredSkills, RequiredSkillNames = "C#, SQL" });

        var result = await _service.PreviewAsync(request);

        result.PassingCount.Should().Be(1);
    }

    [Fact]
    public async Task PreviewAsync_DistrictCriterion_CaseInsensitiveSubstringMatch_ShouldPass()
    {
        var profile = Profile("a@x.com", presentAddress: "House 12, Gulshan, DHAKA");
        SetupApplicationsAndProfiles(new List<JobApplication> { Application(1, "a@x.com") }, new List<CandidateProfile> { profile });

        var request = DefinitionRequest(FilterCombinatorEnum.And,
            new ShortlistFilterCriterionRequest { CriterionType = CriterionTypeEnum.District, RequiredDistrict = "dhaka" });

        var result = await _service.PreviewAsync(request);

        result.PassingCount.Should().Be(1);
    }

    [Fact]
    public async Task PreviewAsync_DistrictCriterion_NoAddress_ShouldFail()
    {
        var profile = Profile("a@x.com", presentAddress: null);
        SetupApplicationsAndProfiles(new List<JobApplication> { Application(1, "a@x.com") }, new List<CandidateProfile> { profile });

        var request = DefinitionRequest(FilterCombinatorEnum.And,
            new ShortlistFilterCriterionRequest { CriterionType = CriterionTypeEnum.District, RequiredDistrict = "Dhaka" });

        var result = await _service.PreviewAsync(request);

        result.PassingCount.Should().Be(0);
    }

    [Fact]
    public async Task PreviewAsync_MinScreeningScoreCriterion_AlwaysUnmet()
    {
        // No score field exists anywhere on JobApplication yet - documented, intentional gap.
        var profile = Profile("a@x.com");
        SetupApplicationsAndProfiles(new List<JobApplication> { Application(1, "a@x.com") }, new List<CandidateProfile> { profile });

        var request = DefinitionRequest(FilterCombinatorEnum.And,
            new ShortlistFilterCriterionRequest { CriterionType = CriterionTypeEnum.MinScreeningScore, MinScreeningScore = 0 });

        var result = await _service.PreviewAsync(request);

        result.PassingCount.Should().Be(0);
    }

    [Fact]
    public async Task PreviewAsync_AndCombinator_OneCriterionFailing_ShouldFailOverall()
    {
        var profile = Profile("a@x.com", presentAddress: "Dhaka", educations: new List<CandidateEducation> { new() { EducationLevel = EducationLevelEnum.SSC } });
        SetupApplicationsAndProfiles(new List<JobApplication> { Application(1, "a@x.com") }, new List<CandidateProfile> { profile });

        var request = DefinitionRequest(FilterCombinatorEnum.And,
            new ShortlistFilterCriterionRequest { CriterionType = CriterionTypeEnum.District, RequiredDistrict = "Dhaka" },
            new ShortlistFilterCriterionRequest { CriterionType = CriterionTypeEnum.EducationLevel, MinEducationLevel = EducationLevelEnum.Bachelor });

        var result = await _service.PreviewAsync(request);

        result.PassingCount.Should().Be(0);
    }

    [Fact]
    public async Task PreviewAsync_OrCombinator_OneCriterionPassing_ShouldPassOverall()
    {
        var profile = Profile("a@x.com", presentAddress: "Dhaka", educations: new List<CandidateEducation> { new() { EducationLevel = EducationLevelEnum.SSC } });
        SetupApplicationsAndProfiles(new List<JobApplication> { Application(1, "a@x.com") }, new List<CandidateProfile> { profile });

        var request = DefinitionRequest(FilterCombinatorEnum.Or,
            new ShortlistFilterCriterionRequest { CriterionType = CriterionTypeEnum.District, RequiredDistrict = "Dhaka" },
            new ShortlistFilterCriterionRequest { CriterionType = CriterionTypeEnum.EducationLevel, MinEducationLevel = EducationLevelEnum.Bachelor });

        var result = await _service.PreviewAsync(request);

        result.PassingCount.Should().Be(1);
    }

    [Fact]
    public async Task PreviewAsync_ApplicationWithNoMatchingProfile_ShouldFailWithoutThrowing()
    {
        SetupApplicationsAndProfiles(new List<JobApplication> { Application(1, "nobody@x.com") }, new List<CandidateProfile>());

        var request = DefinitionRequest(FilterCombinatorEnum.And,
            new ShortlistFilterCriterionRequest { CriterionType = CriterionTypeEnum.MinExperienceYears, MinExperienceYears = 0 });

        var act = () => _service.PreviewAsync(request);

        var result = await act.Should().NotThrowAsync();
        result.Subject.PassingCount.Should().Be(0);
        result.Subject.TotalApplications.Should().Be(1);
    }

    [Fact]
    public async Task PreviewAsync_MixedBatch_ShouldReturnCorrectTotalAndPassingCounts()
    {
        var passing = Profile("pass@x.com", workExperiences: new List<CandidateWorkExperience> { new() { StartDate = DateTime.UtcNow.AddYears(-5), EndDate = null, IsCurrent = true } });
        var failing = Profile("fail@x.com", workExperiences: new List<CandidateWorkExperience> { new() { StartDate = DateTime.UtcNow.AddYears(-1), EndDate = null, IsCurrent = true } });

        SetupApplicationsAndProfiles(
            new List<JobApplication> { Application(1, "pass@x.com"), Application(2, "fail@x.com"), Application(3, "unknown@x.com") },
            new List<CandidateProfile> { passing, failing });

        var request = DefinitionRequest(FilterCombinatorEnum.And,
            new ShortlistFilterCriterionRequest { CriterionType = CriterionTypeEnum.MinExperienceYears, MinExperienceYears = 3 });

        var result = await _service.PreviewAsync(request);

        result.TotalApplications.Should().Be(3);
        result.PassingCount.Should().Be(1);
        result.PassingJobApplicationIds.Should().Equal(new List<long> { 1 });
    }

    [Fact]
    public async Task PreviewAsync_SavedFilterMode_ShouldLoadCriteriaFromRepository()
    {
        var filter = new ShortlistFilter
        {
            ShortlistFilterId = 5,
            CombineWith = FilterCombinatorEnum.And,
            Criteria = new List<ShortlistFilterCriterion>
            {
                new() { CriterionType = CriterionTypeEnum.MinExperienceYears, MinExperienceYears = 1 }
            }
        };
        _shortlistFilterRepositoryMock.Setup(r => r.GetByIdWithCriteriaAsync(5)).ReturnsAsync(filter);

        var profile = Profile("a@x.com", workExperiences: new List<CandidateWorkExperience> { new() { StartDate = DateTime.UtcNow.AddYears(-2), EndDate = null, IsCurrent = true } });
        SetupApplicationsAndProfiles(new List<JobApplication> { Application(1, "a@x.com") }, new List<CandidateProfile> { profile });

        var request = new ShortlistFilterPreviewRequest { ShortlistFilterId = 5, JobPostingId = 1 };

        var result = await _service.PreviewAsync(request);

        result.PassingCount.Should().Be(1);
        _shortlistFilterRepositoryMock.Verify(r => r.GetByIdWithCriteriaAsync(5), Times.Once);
    }

    [Fact]
    public async Task PreviewAsync_UnsavedDefinitionMode_ShouldNotQuerySavedFilterRepository()
    {
        SetupApplicationsAndProfiles(new List<JobApplication>(), new List<CandidateProfile>());

        var request = DefinitionRequest(FilterCombinatorEnum.And,
            new ShortlistFilterCriterionRequest { CriterionType = CriterionTypeEnum.MinExperienceYears, MinExperienceYears = 1 });

        await _service.PreviewAsync(request);

        _shortlistFilterRepositoryMock.Verify(r => r.GetByIdWithCriteriaAsync(It.IsAny<long>()), Times.Never);
    }
}
