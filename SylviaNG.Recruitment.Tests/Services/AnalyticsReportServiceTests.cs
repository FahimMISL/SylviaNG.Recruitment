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
    private readonly Mock<IInterviewEvaluationRepository> _interviewEvaluationRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly AnalyticsReportService _service;

    public AnalyticsReportServiceTests()
    {
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _applicationStatusHistoryRepositoryMock = new Mock<IApplicationStatusHistoryRepository>();
        _offerLetterRepositoryMock = new Mock<IOfferLetterRepository>();
        _candidateProfileRepositoryMock = new Mock<ICandidateProfileRepository>();
        _interviewEvaluationRepositoryMock = new Mock<IInterviewEvaluationRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();

        _service = new AnalyticsReportService(
            _jobApplicationRepositoryMock.Object,
            _applicationStatusHistoryRepositoryMock.Object,
            _offerLetterRepositoryMock.Object,
            _candidateProfileRepositoryMock.Object,
            _interviewEvaluationRepositoryMock.Object,
            _employeeRepositoryMock.Object);
    }

    private static JobApplication MakeApplication(
        long id, long jobPostingId = 1, string email = "candidate@example.com",
        ApplicationStatusEnum status = ApplicationStatusEnum.Applied,
        ApplicationSourceEnum source = ApplicationSourceEnum.External,
        ReferralSource? referralSource = null)
    {
        return new JobApplication
        {
            JobApplicationId = id,
            JobPostingId = jobPostingId,
            CandidateEmail = email,
            ApplicationStatus = status,
            Source = source,
            ReferralSource = referralSource,
            JobPosting = new JobPosting { JobPostingId = jobPostingId, PostingDate = new DateTime(2026, 1, 1) }
        };
    }

    private static InterviewEvaluation MakeEvaluation(
        long id, long employeeId, decimal score, decimal maxScore = 100, decimal weight = 1,
        EvaluationRecommendationEnum? recommendation = null)
    {
        var criterion = new ScorecardCriterion { ScorecardCriterionId = 1, Weight = weight, MaxScore = maxScore, Name = "Overall" };
        return new InterviewEvaluation
        {
            InterviewEvaluationId = id,
            EmployeeId = employeeId,
            Recommendation = recommendation,
            SubmittedAt = new DateTime(2026, 1, 1),
            Scorecard = new Scorecard { ScorecardId = 1, Criteria = new List<ScorecardCriterion> { criterion } },
            Scores = new List<InterviewEvaluationScore> { new() { ScorecardCriterionId = 1, Score = score } }
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

    // ── GetCandidateSourceAnalyticsAsync (US-108) ──────────────────────────

    [Fact]
    public async Task GetCandidateSourceAnalyticsAsync_GroupsByReferralSourceName_FallsBackToApplicationSourceLabel()
    {
        var referral = new ReferralSource { ReferralSourceId = 1, Name = "Employee Referral" };
        var applications = new List<JobApplication>
        {
            MakeApplication(1, referralSource: referral),
            MakeApplication(2, source: ApplicationSourceEnum.External), // no ReferralSource -> falls back to "Career Portal"
            MakeApplication(3, source: ApplicationSourceEnum.Internal), // falls back to "Internal"
        };

        _jobApplicationRepositoryMock
            .Setup(r => r.GetForAnalyticsScopeAsync(null, null, null, null))
            .ReturnsAsync(applications);

        var result = await _service.GetCandidateSourceAnalyticsAsync(new CandidateSourceAnalyticsRequest());

        result.Segments.Should().ContainSingle(s => s.SourceLabel == "Employee Referral" && s.TotalApplications == 1);
        result.Segments.Should().ContainSingle(s => s.SourceLabel == "Career Portal" && s.TotalApplications == 1);
        result.Segments.Should().ContainSingle(s => s.SourceLabel == "Internal" && s.TotalApplications == 1);
    }

    [Fact]
    public async Task GetCandidateSourceAnalyticsAsync_ComputesShortlistedHiredAndConversionRate()
    {
        var referral = new ReferralSource { ReferralSourceId = 1, Name = "BDJobs" };
        var applications = new List<JobApplication>
        {
            MakeApplication(1, referralSource: referral, status: ApplicationStatusEnum.Applied),
            MakeApplication(2, referralSource: referral, status: ApplicationStatusEnum.Shortlisted),
            MakeApplication(3, referralSource: referral, status: ApplicationStatusEnum.Hired),
            MakeApplication(4, referralSource: referral, status: ApplicationStatusEnum.Hired),
        };

        _jobApplicationRepositoryMock
            .Setup(r => r.GetForAnalyticsScopeAsync(null, null, null, null))
            .ReturnsAsync(applications);

        var result = await _service.GetCandidateSourceAnalyticsAsync(new CandidateSourceAnalyticsRequest());

        var segment = result.Segments.Single(s => s.SourceLabel == "BDJobs");
        segment.TotalApplications.Should().Be(4);
        // Shortlisted+Hired both count as "reached Shortlisted or beyond" -> 3 of 4.
        segment.ShortlistedCount.Should().Be(3);
        segment.HiredCount.Should().Be(2);
        segment.ConversionRatePercent.Should().BeApproximately(50.0, 0.1);
    }

    [Fact]
    public async Task GetCandidateSourceAnalyticsAsync_EmploymentTypeFilter_ShouldExcludeNonMatchingVacancies()
    {
        var fullTimeApp = MakeApplication(1);
        fullTimeApp.JobPosting.EmploymentType = EmploymentTypeEnum.FullTime;
        var contractApp = MakeApplication(2);
        contractApp.JobPosting.EmploymentType = EmploymentTypeEnum.Contract;

        _jobApplicationRepositoryMock
            .Setup(r => r.GetForAnalyticsScopeAsync(null, null, null, null))
            .ReturnsAsync(new List<JobApplication> { fullTimeApp, contractApp });

        var result = await _service.GetCandidateSourceAnalyticsAsync(
            new CandidateSourceAnalyticsRequest { EmploymentType = EmploymentTypeEnum.FullTime });

        result.Segments.Sum(s => s.TotalApplications).Should().Be(1);
    }

    // ── GetInterviewAnalyticsAsync (US-110) ──────────────────────────

    [Fact]
    public async Task GetInterviewAnalyticsAsync_ComputesPerPanelistAverageScoreAndRecommendationCounts()
    {
        var evaluations = new List<InterviewEvaluation>
        {
            MakeEvaluation(1, employeeId: 10, score: 80, recommendation: EvaluationRecommendationEnum.Recommended),
            MakeEvaluation(2, employeeId: 10, score: 60, recommendation: EvaluationRecommendationEnum.NotRecommended),
            MakeEvaluation(3, employeeId: 20, score: 90, recommendation: EvaluationRecommendationEnum.OnHold),
        };

        _interviewEvaluationRepositoryMock
            .Setup(r => r.GetForAnalyticsScopeAsync(null, null, null, null))
            .ReturnsAsync(evaluations);
        _employeeRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>()))
            .ReturnsAsync(new List<Employee>
            {
                new() { EmployeeId = 10, EmployeeName = "Alice" },
                new() { EmployeeId = 20, EmployeeName = "Bob" },
            });

        var result = await _service.GetInterviewAnalyticsAsync(new InterviewAnalyticsRequest());

        var alice = result.Panelists.Single(p => p.EmployeeId == 10);
        alice.EmployeeName.Should().Be("Alice");
        alice.InterviewsConducted.Should().Be(2);
        alice.AverageScore.Should().Be(70.0m);
        alice.RecommendedCount.Should().Be(1);
        alice.NotRecommendedCount.Should().Be(1);
        alice.OnHoldCount.Should().Be(0);

        var bob = result.Panelists.Single(p => p.EmployeeId == 20);
        bob.EmployeeName.Should().Be("Bob");
        bob.OnHoldCount.Should().Be(1);
    }

    [Fact]
    public async Task GetInterviewAnalyticsAsync_MissingEmployeeRecord_ShouldFallBackToPlaceholderName()
    {
        var evaluations = new List<InterviewEvaluation> { MakeEvaluation(1, employeeId: 99, score: 50) };

        _interviewEvaluationRepositoryMock
            .Setup(r => r.GetForAnalyticsScopeAsync(null, null, null, null))
            .ReturnsAsync(evaluations);
        _employeeRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>()))
            .ReturnsAsync(new List<Employee>());

        var result = await _service.GetInterviewAnalyticsAsync(new InterviewAnalyticsRequest());

        result.Panelists.Single().EmployeeName.Should().Be("Employee 99");
    }

    [Fact]
    public async Task GetInterviewAnalyticsAsync_ShouldBucketWeightedScoresIntoFiveHistogramBands()
    {
        var evaluations = new List<InterviewEvaluation>
        {
            MakeEvaluation(1, employeeId: 1, score: 10),  // 0-20 band
            MakeEvaluation(2, employeeId: 1, score: 55),  // 41-60 band
            MakeEvaluation(3, employeeId: 1, score: 95),  // 81-100 band
        };

        _interviewEvaluationRepositoryMock
            .Setup(r => r.GetForAnalyticsScopeAsync(null, null, null, null))
            .ReturnsAsync(evaluations);
        _employeeRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>()))
            .ReturnsAsync(new List<Employee> { new() { EmployeeId = 1, EmployeeName = "Alice" } });

        var result = await _service.GetInterviewAnalyticsAsync(new InterviewAnalyticsRequest());

        result.ScoreHistogram.Single(b => b.Band == "0-20").Count.Should().Be(1);
        result.ScoreHistogram.Single(b => b.Band == "41-60").Count.Should().Be(1);
        result.ScoreHistogram.Single(b => b.Band == "81-100").Count.Should().Be(1);
        result.ScoreHistogram.Single(b => b.Band == "21-40").Count.Should().Be(0);
        result.ScoreHistogram.Single(b => b.Band == "61-80").Count.Should().Be(0);
    }
}
