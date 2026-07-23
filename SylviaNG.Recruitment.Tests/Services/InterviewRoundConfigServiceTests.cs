using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class InterviewRoundConfigServiceTests
{
    private readonly Mock<IInterviewRoundConfigRepository> _interviewRoundConfigRepositoryMock;
    private readonly Mock<IJobPostingRepository> _jobPostingRepositoryMock;
    private readonly Mock<IInterviewRepository> _interviewRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly InterviewRoundConfigService _service;

    public InterviewRoundConfigServiceTests()
    {
        _interviewRoundConfigRepositoryMock = new Mock<IInterviewRoundConfigRepository>();
        _jobPostingRepositoryMock = new Mock<IJobPostingRepository>();
        _interviewRepositoryMock = new Mock<IInterviewRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new InterviewRoundConfigService(
            _interviewRoundConfigRepositoryMock.Object,
            _jobPostingRepositoryMock.Object,
            _interviewRepositoryMock.Object,
            _employeeRepositoryMock.Object,
            _unitOfWorkMock.Object);

        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(100)).ReturnsAsync(new JobPosting { JobPostingId = 100 });
        _interviewRoundConfigRepositoryMock.Setup(r => r.GetAllByJobPostingIdAsync(100)).ReturnsAsync(new List<InterviewRoundConfig>());
    }

    [Fact]
    public async Task ReplaceForJobPostingAsync_UnknownJobPosting_ShouldThrowNotFoundException()
    {
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((JobPosting?)null);

        var act = () => _service.ReplaceForJobPostingAsync(999, new InterviewRoundConfigReplaceRequest());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ReplaceForJobPostingAsync_UnknownPanelistEmployee_ShouldThrowNotFoundException()
    {
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Employee?)null);

        var request = new InterviewRoundConfigReplaceRequest
        {
            Rounds = new List<InterviewRoundConfigRequest>
            {
                new() { Name = "Technical Round 1", Sequence = 1, PanelistEmployeeIds = new List<long> { 1 } },
            },
        };

        var act = () => _service.ReplaceForJobPostingAsync(100, request);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ReplaceForJobPostingAsync_NewRounds_ShouldNormalizeSequenceAndAdd()
    {
        var request = new InterviewRoundConfigReplaceRequest
        {
            Rounds = new List<InterviewRoundConfigRequest>
            {
                new() { Name = "HR Round", Sequence = 5 },
                new() { Name = "Technical Round 1", Sequence = 1 },
            },
        };

        var added = new List<InterviewRoundConfig>();
        _interviewRoundConfigRepositoryMock.Setup(r => r.AddAsync(It.IsAny<InterviewRoundConfig>()))
            .Callback<InterviewRoundConfig>(added.Add)
            .Returns(Task.CompletedTask);

        await _service.ReplaceForJobPostingAsync(100, request);

        added.Should().HaveCount(2);
        added.Single(a => a.Name == "Technical Round 1").Sequence.Should().Be(1);
        added.Single(a => a.Name == "HR Round").Sequence.Should().Be(2);
        added.Should().OnlyContain(a => a.JobPostingId == 100);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ReplaceForJobPostingAsync_RemovingRoundReferencedByInterview_ShouldThrowResourceInUseException()
    {
        var existingRound = new InterviewRoundConfig { InterviewRoundConfigId = 1, JobPostingId = 100, Name = "Technical Round 1", Sequence = 1 };
        _interviewRoundConfigRepositoryMock.Setup(r => r.GetAllByJobPostingIdAsync(100)).ReturnsAsync(new List<InterviewRoundConfig> { existingRound });
        _interviewRepositoryMock.Setup(r => r.ExistsForRoundConfigAsync(1)).ReturnsAsync(true);

        var request = new InterviewRoundConfigReplaceRequest { Rounds = new List<InterviewRoundConfigRequest>() };

        var act = () => _service.ReplaceForJobPostingAsync(100, request);

        await act.Should().ThrowAsync<ResourceInUseException>();
        _interviewRoundConfigRepositoryMock.Verify(r => r.Delete(It.IsAny<InterviewRoundConfig>()), Times.Never);
    }

    [Fact]
    public async Task ReplaceForJobPostingAsync_RemovingUnreferencedRound_ShouldDelete()
    {
        var existingRound = new InterviewRoundConfig { InterviewRoundConfigId = 1, JobPostingId = 100, Name = "Technical Round 1", Sequence = 1 };
        _interviewRoundConfigRepositoryMock.Setup(r => r.GetAllByJobPostingIdAsync(100)).ReturnsAsync(new List<InterviewRoundConfig> { existingRound });
        _interviewRepositoryMock.Setup(r => r.ExistsForRoundConfigAsync(1)).ReturnsAsync(false);

        var request = new InterviewRoundConfigReplaceRequest { Rounds = new List<InterviewRoundConfigRequest>() };

        await _service.ReplaceForJobPostingAsync(100, request);

        _interviewRoundConfigRepositoryMock.Verify(r => r.Delete(existingRound), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
