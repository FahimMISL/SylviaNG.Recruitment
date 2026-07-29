using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Features.Analytics.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Tests.Services;

public class AnalyticsReportServiceTests
{
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<IApplicationStatusHistoryRepository> _applicationStatusHistoryRepositoryMock;
    private readonly Mock<IOfferLetterRepository> _offerLetterRepositoryMock;
    private readonly Mock<ICandidateProfileRepository> _candidateProfileRepositoryMock;
    private readonly AnalyticsReportService _service;

    public AnalyticsReportServiceTests()
    {
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _applicationStatusHistoryRepositoryMock = new Mock<IApplicationStatusHistoryRepository>();
        _offerLetterRepositoryMock = new Mock<IOfferLetterRepository>();
        _candidateProfileRepositoryMock = new Mock<ICandidateProfileRepository>();

        _service = new AnalyticsReportService(
            _jobApplicationRepositoryMock.Object,
            _applicationStatusHistoryRepositoryMock.Object,
            _offerLetterRepositoryMock.Object,
            _candidateProfileRepositoryMock.Object);
    }

    private static JobApplication MakeApplication(long id, long jobPostingId = 1, string email = "candidate@example.com")
    {
        return new JobApplication
        {
            JobApplicationId = id,
            JobPostingId = jobPostingId,
            CandidateEmail = email,
            JobPosting = new JobPosting { JobPostingId = jobPostingId, PostingDate = new DateTime(2026, 1, 1) }
        };
    }

    private static ApplicationStatusHistory MakeHistory(
        long appId, ApplicationStatusEnum toStatus, DateTime changedAt, ApplicationStatusReason? reason = null)
    {
        return new ApplicationStatusHistory
        {
            JobApplicationId = appId,
            ToStatus = toStatus,
            ChangedAt = changedAt,
            Reason = reason,
            ReasonId = reason?.ApplicationStatusReasonId
        };
    }

    // ── GetRecruitmentFunnelAsync (US-106) ──────────────────────────

    [Fact]
    public async Task GetRecruitmentFunnelAsync_AppliedCount_IncludesApplicationsWithNoHistoryRows()
    {
        // Applied is never reliably logged as a history row (only waiver-matched submissions get
        // one) - every in-scope application must still count toward the Applied stage.
        var applications = new List<JobApplication> { MakeApplication(1), MakeApplication(2) };

        _jobApplicationRepositoryMock
            .Setup(r => r.GetForAnalyticsScopeAsync(null, null, null, null))
            .ReturnsAsync(applications);
        _applicationStatusHistoryRepositoryMock
            .Setup(r => r.GetForApplicationsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<ApplicationStatusHistory>());

        var result = await _service.GetRecruitmentFunnelAsync(new RecruitmentFunnelRequest());

        var applied = result.Stages.Single(s => s.Status == ApplicationStatusEnum.Applied);
        applied.Count.Should().Be(2);
        applied.ConversionFromPreviousPercent.Should().BeNull();
    }

    [Fact]
    public async Task GetRecruitmentFunnelAsync_ShouldComputeCountsAndConversionPercentPerStage()
    {
        var applications = new List<JobApplication> { MakeApplication(1), MakeApplication(2), MakeApplication(3) };
        var history = new List<ApplicationStatusHistory>
        {
            MakeHistory(1, ApplicationStatusEnum.Screening, new DateTime(2026, 1, 2)),
            MakeHistory(1, ApplicationStatusEnum.Shortlisted, new DateTime(2026, 1, 3)),
            MakeHistory(2, ApplicationStatusEnum.Screening, new DateTime(2026, 1, 2)),
        };

        _jobApplicationRepositoryMock
            .Setup(r => r.GetForAnalyticsScopeAsync(null, null, null, null))
            .ReturnsAsync(applications);
        _applicationStatusHistoryRepositoryMock
            .Setup(r => r.GetForApplicationsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(history);

        var result = await _service.GetRecruitmentFunnelAsync(new RecruitmentFunnelRequest());

        result.Stages.Single(s => s.Status == ApplicationStatusEnum.Applied).Count.Should().Be(3);
        result.Stages.Single(s => s.Status == ApplicationStatusEnum.Screening).Count.Should().Be(2);
        result.Stages.Single(s => s.Status == ApplicationStatusEnum.Screening).ConversionFromPreviousPercent
            .Should().BeApproximately(66.7, 0.1);
        result.Stages.Single(s => s.Status == ApplicationStatusEnum.Shortlisted).Count.Should().Be(1);
        result.Stages.Single(s => s.Status == ApplicationStatusEnum.Shortlisted).ConversionFromPreviousPercent
            .Should().BeApproximately(50.0, 0.1);
        // InterviewScheduled's count hits 0 first (previous=1 -> conversion 0.0%); every stage
        // after that has a 0-count denominator, so conversion is undefined (null), not 0%.
        result.Stages.Single(s => s.Status == ApplicationStatusEnum.InterviewScheduled).Count.Should().Be(0);
        result.Stages.Single(s => s.Status == ApplicationStatusEnum.InterviewScheduled).ConversionFromPreviousPercent
            .Should().Be(0.0);
        result.Stages.Single(s => s.Status == ApplicationStatusEnum.Hired).Count.Should().Be(0);
        result.Stages.Single(s => s.Status == ApplicationStatusEnum.Hired).ConversionFromPreviousPercent
            .Should().BeNull();
    }

    [Fact]
    public async Task GetRecruitmentFunnelAsync_ZeroApplicationsAtAStage_ShouldNotThrowOnDivideByZero()
    {
        _jobApplicationRepositoryMock
            .Setup(r => r.GetForAnalyticsScopeAsync(null, null, null, null))
            .ReturnsAsync(new List<JobApplication>());
        _applicationStatusHistoryRepositoryMock
            .Setup(r => r.GetForApplicationsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<ApplicationStatusHistory>());

        var act = async () => await _service.GetRecruitmentFunnelAsync(new RecruitmentFunnelRequest());

        (await act.Should().NotThrowAsync()).Which.Stages.Should().OnlyContain(s => s.Count == 0);
    }

    [Fact]
    public async Task GetRecruitmentFunnelAsync_DropOffReasons_OnlyCountRejectedOrWithdrawnRowsWithReasonRecorded()
    {
        var applications = new List<JobApplication> { MakeApplication(1), MakeApplication(2), MakeApplication(3) };
        var reason = new ApplicationStatusReason { ApplicationStatusReasonId = 10, Label = "Failed assessment" };
        var history = new List<ApplicationStatusHistory>
        {
            MakeHistory(1, ApplicationStatusEnum.Screening, new DateTime(2026, 1, 2)),
            MakeHistory(1, ApplicationStatusEnum.Rejected, new DateTime(2026, 1, 3), reason),

            MakeHistory(2, ApplicationStatusEnum.Screening, new DateTime(2026, 1, 2)),
            MakeHistory(2, ApplicationStatusEnum.Rejected, new DateTime(2026, 1, 3)), // no reason recorded

            MakeHistory(3, ApplicationStatusEnum.Screening, new DateTime(2026, 1, 2)),
            MakeHistory(3, ApplicationStatusEnum.Shortlisted, new DateTime(2026, 1, 3)), // still progressing
        };

        _jobApplicationRepositoryMock
            .Setup(r => r.GetForAnalyticsScopeAsync(null, null, null, null))
            .ReturnsAsync(applications);
        _applicationStatusHistoryRepositoryMock
            .Setup(r => r.GetForApplicationsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(history);

        var result = await _service.GetRecruitmentFunnelAsync(new RecruitmentFunnelRequest());

        var screening = result.Stages.Single(s => s.Status == ApplicationStatusEnum.Screening);
        screening.DroppedCount.Should().Be(2);
        screening.PassedCount.Should().Be(1);
        screening.DropOffReasons.Should().ContainSingle(r => r.ReasonLabel == "Failed assessment" && r.Count == 1);
    }

    [Fact]
    public async Task GetRecruitmentFunnelAsync_IsInternalFilter_ShouldExcludeNonMatchingCandidates()
    {
        var applications = new List<JobApplication>
        {
            MakeApplication(1, email: "internal@example.com"),
            MakeApplication(2, email: "external@example.com"),
        };

        _jobApplicationRepositoryMock
            .Setup(r => r.GetForAnalyticsScopeAsync(null, null, null, null))
            .ReturnsAsync(applications);
        _candidateProfileRepositoryMock
            .Setup(r => r.GetByEmailsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new List<CandidateProfile>
            {
                new() { CandidateProfileId = 1, Email = "internal@example.com", EmployeeId = 99 },
                new() { CandidateProfileId = 2, Email = "external@example.com" },
            });
        _applicationStatusHistoryRepositoryMock
            .Setup(r => r.GetForApplicationsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<ApplicationStatusHistory>());

        var result = await _service.GetRecruitmentFunnelAsync(new RecruitmentFunnelRequest { IsInternal = true });

        result.Stages.Single(s => s.Status == ApplicationStatusEnum.Applied).Count.Should().Be(1);
    }

    [Fact]
    public async Task ExportRecruitmentFunnelCsvAsync_ShouldProduceHeaderPlusOneRowPerFunnelStage()
    {
        _jobApplicationRepositoryMock
            .Setup(r => r.GetForAnalyticsScopeAsync(null, null, null, null))
            .ReturnsAsync(new List<JobApplication>());
        _applicationStatusHistoryRepositoryMock
            .Setup(r => r.GetForApplicationsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<ApplicationStatusHistory>());

        var csvBytes = await _service.ExportRecruitmentFunnelCsvAsync(new RecruitmentFunnelRequest());
        var csv = System.Text.Encoding.UTF8.GetString(csvBytes);
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(l => l.TrimEnd('\r')).ToArray();

        lines[0].Should().Be("Stage,Count,Conversion From Previous (%),Passed,Dropped,Drop-off Reasons");
        lines.Length.Should().Be(8); // header + 7 funnel stages
    }

    // ── GetTimeToHireAsync (US-107) ──────────────────────────

    [Fact]
    public async Task GetTimeToHireAsync_ShouldComputeAverageMinMaxFromAcceptedOffers()
    {
        var applications = new List<JobApplication> { MakeApplication(1, jobPostingId: 1), MakeApplication(2, jobPostingId: 2) };
        var offers = new List<OfferLetter>
        {
            new()
            {
                OfferLetterId = 1, JobApplicationId = 1, DecisionAt = new DateTime(2026, 1, 11),
                JobApplication = applications[0]
            },
            new()
            {
                OfferLetterId = 2, JobApplicationId = 2, DecisionAt = new DateTime(2026, 1, 21),
                JobApplication = applications[1]
            },
        };

        _jobApplicationRepositoryMock
            .Setup(r => r.GetForAnalyticsScopeAsync(null, null, null, null))
            .ReturnsAsync(applications);
        _offerLetterRepositoryMock
            .Setup(r => r.GetAcceptedForApplicationsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(offers);
        _applicationStatusHistoryRepositoryMock
            .Setup(r => r.GetForApplicationsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<ApplicationStatusHistory>());

        var result = await _service.GetTimeToHireAsync(new TimeToHireRequest());

        // JobPosting.PostingDate = 2026-01-01 for both; offers decided 10 and 20 days later.
        result.MinDays.Should().Be(10);
        result.MaxDays.Should().Be(20);
        result.AverageDays.Should().Be(15.0);
        result.VacancyCount.Should().Be(2);
    }

    [Fact]
    public async Task GetTimeToHireAsync_NoAcceptedOffers_ShouldReturnNullSummaryNotThrow()
    {
        _jobApplicationRepositoryMock
            .Setup(r => r.GetForAnalyticsScopeAsync(null, null, null, null))
            .ReturnsAsync(new List<JobApplication>());
        _offerLetterRepositoryMock
            .Setup(r => r.GetAcceptedForApplicationsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<OfferLetter>());

        var result = await _service.GetTimeToHireAsync(new TimeToHireRequest());

        result.AverageDays.Should().BeNull();
        result.MinDays.Should().BeNull();
        result.MaxDays.Should().BeNull();
        result.VacancyCount.Should().Be(0);
        result.StageBreakdown.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTimeToHireAsync_StageBreakdown_ShouldAverageConsecutiveChangedAtDeltas()
    {
        var application = MakeApplication(1);
        var offers = new List<OfferLetter>
        {
            new() { OfferLetterId = 1, JobApplicationId = 1, DecisionAt = new DateTime(2026, 1, 20), JobApplication = application }
        };
        var history = new List<ApplicationStatusHistory>
        {
            MakeHistory(1, ApplicationStatusEnum.Screening, new DateTime(2026, 1, 3)), // +2 days from Applied baseline not tracked, so first row has no "previous" -> skipped
            MakeHistory(1, ApplicationStatusEnum.Shortlisted, new DateTime(2026, 1, 6)), // +3 days from Screening row
        };

        _jobApplicationRepositoryMock
            .Setup(r => r.GetForAnalyticsScopeAsync(null, null, null, null))
            .ReturnsAsync(new List<JobApplication> { application });
        _offerLetterRepositoryMock
            .Setup(r => r.GetAcceptedForApplicationsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(offers);
        _applicationStatusHistoryRepositoryMock
            .Setup(r => r.GetForApplicationsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(history);

        var result = await _service.GetTimeToHireAsync(new TimeToHireRequest());

        result.StageBreakdown.Should().ContainSingle(s => s.ToStatus == ApplicationStatusEnum.Shortlisted && s.AverageDays == 3.0 && s.SampleSize == 1);
        result.StageBreakdown.Should().NotContain(s => s.ToStatus == ApplicationStatusEnum.Screening);
    }

    [Fact]
    public async Task ExportTimeToHireCsvAsync_ShouldProduceHeaderPlusSummaryRows()
    {
        _jobApplicationRepositoryMock
            .Setup(r => r.GetForAnalyticsScopeAsync(null, null, null, null))
            .ReturnsAsync(new List<JobApplication>());
        _offerLetterRepositoryMock
            .Setup(r => r.GetAcceptedForApplicationsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<OfferLetter>());

        var csvBytes = await _service.ExportTimeToHireCsvAsync(new TimeToHireRequest());
        var csv = System.Text.Encoding.UTF8.GetString(csvBytes);
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(l => l.TrimEnd('\r')).ToArray();

        lines[0].Should().Be("Section,Label,Average Days,Sample Size");
        lines.Length.Should().Be(5); // header + 4 summary rows, no stage-breakdown rows
    }
}
