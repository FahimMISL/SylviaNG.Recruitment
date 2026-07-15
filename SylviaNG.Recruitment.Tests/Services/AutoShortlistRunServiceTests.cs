using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.AutoShortlisting.Models;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class AutoShortlistRunServiceTests
{
    private readonly Mock<IAutoShortlistRunRepository> _autoShortlistRunRepositoryMock;
    private readonly Mock<IJobPostingRepository> _jobPostingRepositoryMock;
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<ICandidateProfileRepository> _candidateProfileRepositoryMock;
    private readonly Mock<IShortlistScoringService> _scoringServiceMock;
    private readonly Mock<IJobApplicationService> _jobApplicationServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private AutoShortlistRunService _service;

    public AutoShortlistRunServiceTests()
    {
        _autoShortlistRunRepositoryMock = new Mock<IAutoShortlistRunRepository>();
        _jobPostingRepositoryMock = new Mock<IJobPostingRepository>();
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _candidateProfileRepositoryMock = new Mock<ICandidateProfileRepository>();
        _scoringServiceMock = new Mock<IShortlistScoringService>();
        _scoringServiceMock.Setup(s => s.ProviderName).Returns("Manual");
        _jobApplicationServiceMock = new Mock<IJobApplicationService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = BuildService(new GroqSettings { MaxConcurrentRequests = 5 });
    }

    private AutoShortlistRunService BuildService(GroqSettings groqSettings) =>
        new(
            _autoShortlistRunRepositoryMock.Object,
            _jobPostingRepositoryMock.Object,
            _jobApplicationRepositoryMock.Object,
            _candidateProfileRepositoryMock.Object,
            _scoringServiceMock.Object,
            _jobApplicationServiceMock.Object,
            Options.Create(groqSettings),
            _unitOfWorkMock.Object);

    private static JobApplication Application(long id, string? email) =>
        new() { JobApplicationId = id, JobPostingId = 1, CandidateName = $"Candidate {id}", CandidateEmail = email };

    [Fact]
    public async Task RunAsync_UnknownJobPosting_ShouldThrowNotFoundException()
    {
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((JobPosting?)null);

        var act = () => _service.RunAsync(new AutoShortlistRunRequest { JobPostingId = 1, CutoffScore = 70 });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task RunAsync_CutoffOutOfRange_ShouldThrowValidationException()
    {
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new JobPosting { JobPostingId = 1 });

        var act = () => _service.RunAsync(new AutoShortlistRunRequest { JobPostingId = 1, CutoffScore = 150 });

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async Task RunAsync_AiProviderWithNoApiKey_ShouldThrowGroqUnavailableImmediately()
    {
        _scoringServiceMock.Setup(s => s.ProviderName).Returns("Ai");
        _service = BuildService(new GroqSettings { ApiKey = "" });
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new JobPosting { JobPostingId = 1 });

        var act = () => _service.RunAsync(new AutoShortlistRunRequest { JobPostingId = 1, CutoffScore = 70 });

        await act.Should().ThrowAsync<GroqUnavailableException>();
        _jobApplicationRepositoryMock.Verify(r => r.GetAllByJobPostingIdAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task RunAsync_ApplicationWithNoMatchedProfile_ShouldFailWithoutCallingScorer()
    {
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new JobPosting { JobPostingId = 1 });
        _jobApplicationRepositoryMock.Setup(r => r.GetAllByJobPostingIdAsync(1))
            .ReturnsAsync(new List<JobApplication> { Application(1, "nobody@x.com") });
        _candidateProfileRepositoryMock.Setup(r => r.GetByEmailsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new List<CandidateProfile>());

        AutoShortlistRun? saved = null;
        _autoShortlistRunRepositoryMock.Setup(r => r.AddAsync(It.IsAny<AutoShortlistRun>()))
            .Callback<AutoShortlistRun>(r => saved = r)
            .Returns(Task.CompletedTask);

        var result = await _service.RunAsync(new AutoShortlistRunRequest { JobPostingId = 1, CutoffScore = 70 });

        result.TotalFailed.Should().Be(1);
        result.Results.Single().ScoringFailed.Should().BeTrue();
        result.Results.Single().ScoringError.Should().Contain("No candidate profile found");
        _scoringServiceMock.Verify(s => s.ScoreAsync(It.IsAny<JobPosting>(), It.IsAny<CandidateFactService.CandidateFacts>(), It.IsAny<CancellationToken>()), Times.Never);
        saved!.Results.Should().ContainSingle();
    }

    [Fact]
    public async Task RunAsync_MixedSuccessAndFailure_ShouldPersistBothWithoutThrowing()
    {
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new JobPosting { JobPostingId = 1 });
        _jobApplicationRepositoryMock.Setup(r => r.GetAllByJobPostingIdAsync(1))
            .ReturnsAsync(new List<JobApplication> { Application(1, "ok@x.com"), Application(2, "fail@x.com") });
        _candidateProfileRepositoryMock.Setup(r => r.GetByEmailsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new List<CandidateProfile> { new() { Email = "ok@x.com" }, new() { Email = "fail@x.com" } });

        _scoringServiceMock
            .Setup(s => s.ScoreAsync(It.IsAny<JobPosting>(), It.IsAny<CandidateFactService.CandidateFacts>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CandidateScoringResult(90, "Great fit.", false, null));

        AutoShortlistRun? saved = null;
        _autoShortlistRunRepositoryMock.Setup(r => r.AddAsync(It.IsAny<AutoShortlistRun>()))
            .Callback<AutoShortlistRun>(r => { r.AutoShortlistRunId = 1; saved = r; })
            .Returns(Task.CompletedTask);

        var act = () => _service.RunAsync(new AutoShortlistRunRequest { JobPostingId = 1, CutoffScore = 70 });

        var result = await act.Should().NotThrowAsync();
        result.Subject.Results.Should().HaveCount(2);
        saved!.Results.Should().HaveCount(2);
        saved.Provider.Should().Be("Manual");
    }

    [Fact]
    public async Task AdjustCutoffAsync_ShouldOnlyChangeCutoffScore()
    {
        var run = new AutoShortlistRun { AutoShortlistRunId = 1, JobPostingId = 1, CutoffScore = 50, Provider = "Manual", Results = new List<AutoShortlistResult>() };
        _autoShortlistRunRepositoryMock.Setup(r => r.GetByIdWithResultsAsync(1)).ReturnsAsync(run);
        _jobApplicationRepositoryMock.Setup(r => r.GetAllByJobPostingIdAsync(1)).ReturnsAsync(new List<JobApplication>());

        await _service.AdjustCutoffAsync(1, 80);

        run.CutoffScore.Should().Be(80);
        _autoShortlistRunRepositoryMock.Verify(r => r.Update(run), Times.Once);
        _autoShortlistRunRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AutoShortlistRun>()), Times.Never);
    }

    [Fact]
    public async Task OverrideAsync_ShouldFlipFinalIncludedIndependentOfScore()
    {
        var run = new AutoShortlistRun { AutoShortlistRunId = 1, CutoffScore = 70 };
        var result = new AutoShortlistResult { AutoShortlistResultId = 10, JobApplicationId = 1, Score = 20, AutoShortlistRun = run };
        _autoShortlistRunRepositoryMock.Setup(r => r.GetResultByIdAsync(10)).ReturnsAsync(result);
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(Application(1, "a@x.com"));

        var response = await _service.OverrideAsync(10, HrOverrideDecisionEnum.Approved);

        response.Passed.Should().BeFalse(); // score 20 < cutoff 70
        response.FinalIncluded.Should().BeTrue(); // override wins
        _autoShortlistRunRepositoryMock.Verify(r => r.UpdateResult(result), Times.Once);
    }

    [Fact]
    public async Task ApplyAsync_ShouldBulkUpdateExactlyTheFinalIncludedSet()
    {
        var run = new AutoShortlistRun
        {
            AutoShortlistRunId = 1,
            CutoffScore = 70,
            Results = new List<AutoShortlistResult>
            {
                new() { JobApplicationId = 1, Score = 90 }, // passes, no override -> included
                new() { JobApplicationId = 2, Score = 20 }, // fails, no override -> excluded
                new() { JobApplicationId = 3, Score = 90, HrOverrideDecision = HrOverrideDecisionEnum.Rejected }, // passes but overridden out
                new() { JobApplicationId = 4, ScoringFailed = true, HrOverrideDecision = HrOverrideDecisionEnum.Approved }, // failed but overridden in
            }
        };
        _autoShortlistRunRepositoryMock.Setup(r => r.GetByIdWithResultsAsync(1)).ReturnsAsync(run);
        _jobApplicationServiceMock
            .Setup(s => s.BulkUpdateStatusAsync(It.IsAny<JobApplicationBulkStatusUpdateRequest>()))
            .ReturnsAsync(new JobApplicationBulkStatusUpdateResponse { SucceededIds = new List<long> { 1, 4 } });

        var response = await _service.ApplyAsync(1);

        response.TotalProcessed.Should().Be(4);
        response.TotalShortlisted.Should().Be(2);
        _jobApplicationServiceMock.Verify(s => s.BulkUpdateStatusAsync(It.Is<JobApplicationBulkStatusUpdateRequest>(
            r => r.JobApplicationIds.OrderBy(x => x).SequenceEqual(new List<long> { 1, 4 }) && r.ToStatus == ApplicationStatusEnum.Shortlisted)), Times.Once);
    }
}
