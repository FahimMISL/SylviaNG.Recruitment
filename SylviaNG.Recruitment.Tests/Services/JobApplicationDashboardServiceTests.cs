using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Features.Payments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Domain.Events;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Tests.Services;

public class JobApplicationDashboardServiceTests
{
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<IJobApplicationStageProgressRepository> _jobApplicationStageProgressRepositoryMock;
    private readonly Mock<IApplicationSettingService> _applicationSettingServiceMock;
    private readonly Mock<ICandidateProfileRepository> _candidateProfileRepositoryMock;
    private readonly JobApplicationDashboardService _service;

    public JobApplicationDashboardServiceTests()
    {
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _jobApplicationStageProgressRepositoryMock = new Mock<IJobApplicationStageProgressRepository>();
        _applicationSettingServiceMock = new Mock<IApplicationSettingService>();
        _candidateProfileRepositoryMock = new Mock<ICandidateProfileRepository>();

        // EP-14 US-109: AttachStageProgressInfoAsync's default fixtures - no in-flight stage rows,
        // no global stale-days fallback, so existing GetDashboardPagedAsync tests that don't set up
        // stage-progress fixtures see the tracker columns simply stay unset (null/false).
        _jobApplicationStageProgressRepositoryMock
            .Setup(r => r.GetCurrentByJobApplicationIdsAsync(It.IsAny<List<long>>()))
            .ReturnsAsync(new Dictionary<long, JobApplicationStageProgress>());
        _applicationSettingServiceMock.Setup(s => s.GetDefaultStaleDaysThresholdAsync()).ReturnsAsync((int?)null);

        _service = new JobApplicationDashboardService(
            _jobApplicationRepositoryMock.Object,
            _jobApplicationStageProgressRepositoryMock.Object,
            _applicationSettingServiceMock.Object,
            _candidateProfileRepositoryMock.Object);
    }

    // ── GetDashboardMatchingIdsAsync (US-047 AC5) ──────────────────────────

    [Fact]
    public async Task GetDashboardMatchingIdsAsync_ShouldPassFiltersThroughToRepositoryAndReturnIds()
    {
        var expectedIds = new List<long> { 3, 7, 9 };
        _jobApplicationRepositoryMock
            .Setup(r => r.GetAllMatchingIdsAsync(1, ApplicationStatusEnum.Applied, ApplicationSourceEnum.External, null, null))
            .ReturnsAsync(expectedIds);

        var result = await _service.GetDashboardMatchingIdsAsync(new JobApplicationAttributeFilterRequest
        {
            JobPostingId = 1,
            Status = ApplicationStatusEnum.Applied,
            Source = ApplicationSourceEnum.External
        });

        result.Should().Equal(expectedIds);
    }


    // ── MatchesAttributeFilter / ATS candidate-attribute filtering (US-050) ─

    private static CandidateFactService.CandidateFacts Facts(
        int? age = null,
        double experienceYears = 0,
        IEnumerable<string>? skills = null,
        IEnumerable<EducationLevelEnum>? educationLevels = null,
        string address = "",
        IEnumerable<string>? tags = null) =>
        new(age, experienceYears,
            new HashSet<string>(skills ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase),
            new HashSet<EducationLevelEnum>(educationLevels ?? Enumerable.Empty<EducationLevelEnum>()),
            address,
            new HashSet<string>(tags ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase));

    [Fact]
    public void MatchesAttributeFilter_MinEducationLevel_AnyDegreeMeetingMinimum_ShouldMatch()
    {
        var facts = Facts(educationLevels: new[] { EducationLevelEnum.SSC, EducationLevelEnum.Bachelor });
        var filter = new JobApplicationAttributeFilterRequest { MinEducationLevel = EducationLevelEnum.Diploma };

        JobApplicationDashboardService.MatchesAttributeFilter(facts, filter).Should().BeTrue();
    }

    [Fact]
    public void MatchesAttributeFilter_MinEducationLevel_NoDegreeMeetingMinimum_ShouldNotMatch()
    {
        var facts = Facts(educationLevels: new[] { EducationLevelEnum.SSC });
        var filter = new JobApplicationAttributeFilterRequest { MinEducationLevel = EducationLevelEnum.Bachelor };

        JobApplicationDashboardService.MatchesAttributeFilter(facts, filter).Should().BeFalse();
    }

    [Theory]
    [InlineData(2.0, 5.0, 3.0, true)]
    [InlineData(2.0, 5.0, 1.9, false)]
    [InlineData(2.0, 5.0, 5.1, false)]
    public void MatchesAttributeFilter_ExperienceRange_ShouldRespectMinAndMax(double min, double max, double actual, bool expected)
    {
        var facts = Facts(experienceYears: actual);
        var filter = new JobApplicationAttributeFilterRequest { MinExperienceYears = (decimal)min, MaxExperienceYears = (decimal)max };

        JobApplicationDashboardService.MatchesAttributeFilter(facts, filter).Should().Be(expected);
    }

    [Fact]
    public void MatchesAttributeFilter_Skills_MatchesIfAnySelectedSkillPresent()
    {
        var facts = Facts(skills: new[] { "React", "SQL" });
        var filter = new JobApplicationAttributeFilterRequest { Skills = new List<string> { "Node", "React" } };

        JobApplicationDashboardService.MatchesAttributeFilter(facts, filter).Should().BeTrue();
    }

    [Fact]
    public void MatchesAttributeFilter_Skills_NoneOfSelectedSkillsPresent_ShouldNotMatch()
    {
        var facts = Facts(skills: new[] { "React", "SQL" });
        var filter = new JobApplicationAttributeFilterRequest { Skills = new List<string> { "Node", "Java" } };

        JobApplicationDashboardService.MatchesAttributeFilter(facts, filter).Should().BeFalse();
    }

    [Fact]
    public void MatchesAttributeFilter_Tags_MatchesIfAnySelectedTagPresent()
    {
        var facts = Facts(tags: new[] { "Strong Communicator", "Leadership Potential" });
        var filter = new JobApplicationAttributeFilterRequest { Tags = new List<string> { "Fast Learner", "Leadership Potential" } };

        JobApplicationDashboardService.MatchesAttributeFilter(facts, filter).Should().BeTrue();
    }

    [Fact]
    public void MatchesAttributeFilter_Tags_NoneOfSelectedTagsPresent_ShouldNotMatch()
    {
        var facts = Facts(tags: new[] { "Strong Communicator" });
        var filter = new JobApplicationAttributeFilterRequest { Tags = new List<string> { "Fast Learner" } };

        JobApplicationDashboardService.MatchesAttributeFilter(facts, filter).Should().BeFalse();
    }

    [Fact]
    public void MatchesAttributeFilter_AgeRange_OutsideBounds_ShouldNotMatch()
    {
        var facts = Facts(age: 40);
        var filter = new JobApplicationAttributeFilterRequest { MinAge = 25, MaxAge = 35 };

        JobApplicationDashboardService.MatchesAttributeFilter(facts, filter).Should().BeFalse();
    }

    [Fact]
    public void MatchesAttributeFilter_AgeRange_UnknownAge_ShouldNotMatch()
    {
        var facts = Facts(age: null);
        var filter = new JobApplicationAttributeFilterRequest { MinAge = 25 };

        JobApplicationDashboardService.MatchesAttributeFilter(facts, filter).Should().BeFalse();
    }

    [Fact]
    public void MatchesAttributeFilter_Location_CaseInsensitiveSubstringMatch_ShouldMatch()
    {
        var facts = Facts(address: "House 12, Road 5, Dhanmondi, Dhaka");
        var filter = new JobApplicationAttributeFilterRequest { Location = "dhanmondi" };

        JobApplicationDashboardService.MatchesAttributeFilter(facts, filter).Should().BeTrue();
    }

    [Fact]
    public void MatchesAttributeFilter_CombinedFilters_AllMustMatch()
    {
        var facts = Facts(age: 30, experienceYears: 4, skills: new[] { "React" },
            educationLevels: new[] { EducationLevelEnum.Bachelor }, address: "Dhaka");

        var passingFilter = new JobApplicationAttributeFilterRequest
        {
            MinEducationLevel = EducationLevelEnum.Diploma,
            MinExperienceYears = 2,
            Skills = new List<string> { "React" },
            MinAge = 25,
            MaxAge = 35,
            Location = "Dhaka"
        };
        JobApplicationDashboardService.MatchesAttributeFilter(facts, passingFilter).Should().BeTrue();

        var failingFilter = new JobApplicationAttributeFilterRequest
        {
            MinEducationLevel = EducationLevelEnum.Diploma,
            MinExperienceYears = 2,
            Skills = new List<string> { "React" },
            MinAge = 25,
            MaxAge = 35,
            Location = "Chittagong" // only this field disagrees - whole filter should fail
        };
        JobApplicationDashboardService.MatchesAttributeFilter(facts, failingFilter).Should().BeFalse();
    }

    // ── GetDashboardPagedAsync / GetDashboardMatchingIdsAsync (US-050) ──────

    [Fact]
    public async Task GetDashboardPagedAsync_CandidateAttributeFilterWithoutJobPostingId_ShouldThrowValidation()
    {
        var filter = new JobApplicationAttributeFilterRequest { MinAge = 25 };

        var act = () => _service.GetDashboardPagedAsync(new PagedRequest { Page = 1, PageSize = 10 }, filter);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async Task GetDashboardMatchingIdsAsync_CandidateAttributeFilterWithoutJobPostingId_ShouldThrowValidation()
    {
        var filter = new JobApplicationAttributeFilterRequest { Skills = new List<string> { "React" } };

        var act = () => _service.GetDashboardMatchingIdsAsync(filter);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async Task GetDashboardPagedAsync_NoCandidateAttributeFilters_ShouldDelegateToFastSqlPath()
    {
        var filter = new JobApplicationAttributeFilterRequest { JobPostingId = 1, Status = ApplicationStatusEnum.Applied };
        var pagedResult = new PagedResult<JobApplication>
        {
            Data = new List<JobApplication> { new() { JobApplicationId = 1, JobPostingId = 1, CandidateName = "Jane" } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _jobApplicationRepositoryMock
            .Setup(r => r.GetPaginatedAllAsync(It.IsAny<PagedRequest>(), 1, ApplicationStatusEnum.Applied, null, null, null))
            .ReturnsAsync(pagedResult);

        var result = await _service.GetDashboardPagedAsync(new PagedRequest { Page = 1, PageSize = 10 }, filter);

        result.TotalCount.Should().Be(1);
        result.Data.Should().ContainSingle(d => d.JobApplicationId == 1);
        _candidateProfileRepositoryMock.Verify(r => r.GetByEmailsAsync(It.IsAny<IEnumerable<string>>()), Times.Never);
    }

    [Fact]
    public async Task GetDashboardPagedAsync_ShouldAttachCurrentStageAndComputeDaysInStageAndStaleness()
    {
        // Arrange (EP-14 US-109 AC1/AC2)
        var filter = new JobApplicationAttributeFilterRequest { JobPostingId = 1 };
        var pagedResult = new PagedResult<JobApplication>
        {
            Data = new List<JobApplication> { new() { JobApplicationId = 1, JobPostingId = 1, CandidateName = "Jane" } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _jobApplicationRepositoryMock
            .Setup(r => r.GetPaginatedAllAsync(It.IsAny<PagedRequest>(), 1, null, null, null, null))
            .ReturnsAsync(pagedResult);

        var currentStage = new JobApplicationStageProgress
        {
            StageName = "Technical Interview",
            Status = StageProgressStatusEnum.InProgress,
            StageEnteredAt = DateTime.UtcNow.AddDays(-10),
            SlaDaysSnapshot = 5,
            LastUpdatedByUserName = "abir"
        };
        _jobApplicationStageProgressRepositoryMock
            .Setup(r => r.GetCurrentByJobApplicationIdsAsync(It.Is<List<long>>(ids => ids.Contains(1))))
            .ReturnsAsync(new Dictionary<long, JobApplicationStageProgress> { [1] = currentStage });

        // Act
        var result = await _service.GetDashboardPagedAsync(new PagedRequest { Page = 1, PageSize = 10 }, filter);

        // Assert
        var row = result.Data.Should().ContainSingle().Subject;
        row.CurrentStageName.Should().Be("Technical Interview");
        row.AssignedHrUserName.Should().Be("abir");
        row.DaysInCurrentStage.Should().Be(10);
        row.IsStale.Should().BeTrue();
    }

    [Fact]
    public async Task GetDashboardPagedAsync_StaleOnlyFilter_ShouldRouteThroughInMemoryPathAndExcludeNonStaleRows()
    {
        // Arrange
        var filter = new JobApplicationAttributeFilterRequest { StaleOnly = true };
        var applications = new List<JobApplication>
        {
            new() { JobApplicationId = 1, JobPostingId = 1, CandidateName = "Stale Candidate" },
            new() { JobApplicationId = 2, JobPostingId = 1, CandidateName = "Fresh Candidate" }
        };
        _jobApplicationRepositoryMock
            .Setup(r => r.GetAllMatchingAsync(null, null, null, null, null))
            .ReturnsAsync(applications);

        _jobApplicationStageProgressRepositoryMock
            .Setup(r => r.GetCurrentByJobApplicationIdsAsync(It.IsAny<List<long>>()))
            .ReturnsAsync(new Dictionary<long, JobApplicationStageProgress>
            {
                [1] = new() { Status = StageProgressStatusEnum.InProgress, StageEnteredAt = DateTime.UtcNow.AddDays(-10), SlaDaysSnapshot = 5 },
                [2] = new() { Status = StageProgressStatusEnum.InProgress, StageEnteredAt = DateTime.UtcNow.AddDays(-1), SlaDaysSnapshot = 5 }
            });

        // Act
        var result = await _service.GetDashboardPagedAsync(new PagedRequest { Page = 1, PageSize = 10 }, filter);

        // Assert
        result.Data.Should().ContainSingle(r => r.JobApplicationId == 1);
        _jobApplicationRepositoryMock.Verify(r => r.GetPaginatedAllAsync(It.IsAny<PagedRequest>(), It.IsAny<long?>(), It.IsAny<ApplicationStatusEnum?>(), It.IsAny<ApplicationSourceEnum?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Never);
    }

    [Fact]
    public async Task GetDashboardPagedAsync_SortByDaysInCurrentStage_ShouldRouteThroughInMemoryPathAndSort()
    {
        // Arrange (EP-14 US-109 AC3: stage-derived columns aren't native JobApplication columns,
        // so PaginationExtensions' generic SQL sorter can't reach them)
        var applications = new List<JobApplication>
        {
            new() { JobApplicationId = 1, JobPostingId = 1, CandidateName = "Shorter Wait" },
            new() { JobApplicationId = 2, JobPostingId = 1, CandidateName = "Longer Wait" }
        };
        _jobApplicationRepositoryMock
            .Setup(r => r.GetAllMatchingAsync(null, null, null, null, null))
            .ReturnsAsync(applications);

        _jobApplicationStageProgressRepositoryMock
            .Setup(r => r.GetCurrentByJobApplicationIdsAsync(It.IsAny<List<long>>()))
            .ReturnsAsync(new Dictionary<long, JobApplicationStageProgress>
            {
                [1] = new() { Status = StageProgressStatusEnum.InProgress, StageEnteredAt = DateTime.UtcNow.AddDays(-2) },
                [2] = new() { Status = StageProgressStatusEnum.InProgress, StageEnteredAt = DateTime.UtcNow.AddDays(-20) }
            });

        // Act
        var result = await _service.GetDashboardPagedAsync(
            new PagedRequest { Page = 1, PageSize = 10, SortBy = "DaysInCurrentStage", SortDirection = "desc" },
            new JobApplicationAttributeFilterRequest());

        // Assert
        result.Data.Select(r => r.JobApplicationId).Should().ContainInOrder(2L, 1L);
    }

    [Fact]
    public async Task GetDashboardMatchingIdsAsync_WithSkillFilter_ShouldJoinProfilesAndReturnOnlyMatchingIds()
    {
        var applications = new List<JobApplication>
        {
            new() { JobApplicationId = 1, JobPostingId = 1, CandidateEmail = "react-dev@x.com" },
            new() { JobApplicationId = 2, JobPostingId = 1, CandidateEmail = "no-match@x.com" }
        };
        var profiles = new List<CandidateProfile>
        {
            new() { Email = "react-dev@x.com", Skills = new List<CandidateSkill> { new() { SkillName = "React" } } },
            new() { Email = "no-match@x.com", Skills = new List<CandidateSkill> { new() { SkillName = "Java" } } }
        };
        _jobApplicationRepositoryMock
            .Setup(r => r.GetAllByJobPostingAndScalarFiltersAsync(1, null, null, null, null))
            .ReturnsAsync(applications);
        _candidateProfileRepositoryMock
            .Setup(r => r.GetByEmailsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(profiles);

        var filter = new JobApplicationAttributeFilterRequest { JobPostingId = 1, Skills = new List<string> { "React" } };

        var result = await _service.GetDashboardMatchingIdsAsync(filter);

        result.Should().Equal(new List<long> { 1 });
    }


}
