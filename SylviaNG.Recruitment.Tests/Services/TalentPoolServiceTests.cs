using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class TalentPoolServiceTests
{
    private readonly Mock<ITalentPoolRepository> _talentPoolRepositoryMock;
    private readonly Mock<ITalentPoolCandidateRepository> _talentPoolCandidateRepositoryMock;
    private readonly Mock<ICandidateProfileRepository> _candidateProfileRepositoryMock;
    private readonly Mock<IJobPostingRepository> _jobPostingRepositoryMock;
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<IJobApplicationService> _jobApplicationServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly TalentPoolService _service;

    public TalentPoolServiceTests()
    {
        _talentPoolRepositoryMock = new Mock<ITalentPoolRepository>();
        _talentPoolCandidateRepositoryMock = new Mock<ITalentPoolCandidateRepository>();
        _candidateProfileRepositoryMock = new Mock<ICandidateProfileRepository>();
        _jobPostingRepositoryMock = new Mock<IJobPostingRepository>();
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _jobApplicationServiceMock = new Mock<IJobApplicationService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new TalentPoolService(
            _talentPoolRepositoryMock.Object,
            _talentPoolCandidateRepositoryMock.Object,
            _candidateProfileRepositoryMock.Object,
            _jobPostingRepositoryMock.Object,
            _jobApplicationRepositoryMock.Object,
            _jobApplicationServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithUniqueName_ShouldSaveAndReturnId()
    {
        // Arrange
        _talentPoolRepositoryMock.Setup(r => r.ExistsByNameAsync("Senior Engineers 2026")).ReturnsAsync(false);

        TalentPool? saved = null;
        _talentPoolRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TalentPool>()))
            .Callback<TalentPool>(p => { p.TalentPoolId = 7; saved = p; })
            .Returns(Task.CompletedTask);

        // Act
        var id = await _service.CreateAsync(new TalentPoolCreateRequest { Name = "Senior Engineers 2026" });

        // Assert
        id.Should().Be(7);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Senior Engineers 2026");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowDuplicateException()
    {
        // Arrange
        _talentPoolRepositoryMock.Setup(r => r.ExistsByNameAsync("Senior Engineers 2026")).ReturnsAsync(true);

        // Act
        var act = () => _service.CreateAsync(new TalentPoolCreateRequest { Name = "Senior Engineers 2026" });

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
        _talentPoolRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TalentPool>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithJobPostingId_ShouldValidateAndSetLink()
    {
        // Arrange
        _talentPoolRepositoryMock.Setup(r => r.ExistsByNameAsync("Backend Team 2026", null)).ReturnsAsync(false);
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new JobPosting { JobPostingId = 5 });

        TalentPool? saved = null;
        _talentPoolRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TalentPool>()))
            .Callback<TalentPool>(p => { p.TalentPoolId = 8; saved = p; })
            .Returns(Task.CompletedTask);

        // Act
        var id = await _service.CreateAsync(new TalentPoolCreateRequest { Name = "Backend Team 2026", JobPostingId = 5 });

        // Assert
        id.Should().Be(8);
        saved!.JobPostingId.Should().Be(5);
    }

    [Fact]
    public async Task CreateAsync_WithUnknownJobPostingId_ShouldThrowNotFoundException()
    {
        // Arrange
        _talentPoolRepositoryMock.Setup(r => r.ExistsByNameAsync("Backend Team 2026", null)).ReturnsAsync(false);
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(500)).ReturnsAsync((JobPosting?)null);

        // Act
        var act = () => _service.CreateAsync(new TalentPoolCreateRequest { Name = "Backend Team 2026", JobPostingId = 500 });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _talentPoolRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TalentPool>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithUnknownPool_ShouldThrowNotFoundException()
    {
        // Arrange
        _talentPoolRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((TalentPool?)null);

        // Act
        var act = () => _service.UpdateAsync(99, new TalentPoolUpdateRequest { Name = "Renamed" });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_RenamesPool()
    {
        // Arrange
        var pool = new TalentPool { TalentPoolId = 1, Name = "Old Name" };
        _talentPoolRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pool);
        _talentPoolRepositoryMock.Setup(r => r.ExistsByNameAsync("New Name", 1)).ReturnsAsync(false);

        // Act
        await _service.UpdateAsync(1, new TalentPoolUpdateRequest { Name = "New Name" });

        // Assert
        pool.Name.Should().Be("New Name");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_LinksJobPosting()
    {
        // Arrange
        var pool = new TalentPool { TalentPoolId = 1, Name = "Pool" };
        _talentPoolRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pool);
        _talentPoolRepositoryMock.Setup(r => r.ExistsByNameAsync("Pool", 1)).ReturnsAsync(false);
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new JobPosting { JobPostingId = 5 });

        // Act
        await _service.UpdateAsync(1, new TalentPoolUpdateRequest { Name = "Pool", JobPostingId = 5 });

        // Assert
        pool.JobPostingId.Should().Be(5);
    }

    [Fact]
    public async Task UpdateAsync_UnlinksJobPosting()
    {
        // Arrange
        var pool = new TalentPool { TalentPoolId = 1, Name = "Pool", JobPostingId = 5 };
        _talentPoolRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pool);
        _talentPoolRepositoryMock.Setup(r => r.ExistsByNameAsync("Pool", 1)).ReturnsAsync(false);

        // Act
        await _service.UpdateAsync(1, new TalentPoolUpdateRequest { Name = "Pool", JobPostingId = null });

        // Assert
        pool.JobPostingId.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateNameExcludingSelf_ShouldThrowDuplicateException()
    {
        // Arrange
        var pool = new TalentPool { TalentPoolId = 1, Name = "Pool" };
        _talentPoolRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pool);
        _talentPoolRepositoryMock.Setup(r => r.ExistsByNameAsync("Taken Name", 1)).ReturnsAsync(true);

        // Act
        var act = () => _service.UpdateAsync(1, new TalentPoolUpdateRequest { Name = "Taken Name" });

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async Task GetAllAsync_WithJobPostingIdFilter_PassesThroughToRepository()
    {
        // Arrange
        _talentPoolRepositoryMock.Setup(r => r.GetAllWithCandidateCountAsync(5)).ReturnsAsync(new List<TalentPool>
        {
            new() { TalentPoolId = 1, Name = "Pool", JobPostingId = 5 }
        });

        // Act
        var result = await _service.GetAllAsync(5);

        // Assert
        result.Should().HaveCount(1);
        result[0].JobPostingId.Should().Be(5);
        _talentPoolRepositoryMock.Verify(r => r.GetAllWithCandidateCountAsync(5), Times.Once);
    }

    [Fact]
    public async Task AddCandidatesAsync_WithUnknownPool_ShouldThrowNotFoundException()
    {
        // Arrange
        _talentPoolRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((TalentPool?)null);

        // Act
        var act = () => _service.AddCandidatesAsync(99, new TalentPoolCandidateAddRequest { CandidateProfileIds = new List<long> { 1 } });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddCandidatesAsync_SkipsAlreadyInPoolAndNotFoundCandidates()
    {
        // Arrange
        _talentPoolRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new TalentPool { TalentPoolId = 1, Name = "Pool" });
        _talentPoolCandidateRepositoryMock.Setup(r => r.GetExistingCandidateIdsAsync(1, It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<long> { 2 });

        _candidateProfileRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new CandidateProfile { CandidateProfileId = 2 });
        _candidateProfileRepositoryMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(new CandidateProfile { CandidateProfileId = 3 });
        _candidateProfileRepositoryMock.Setup(r => r.GetByIdAsync(404)).ReturnsAsync((CandidateProfile?)null);

        // Act
        var result = await _service.AddCandidatesAsync(1, new TalentPoolCandidateAddRequest
        {
            CandidateProfileIds = new List<long> { 2, 3, 404 }
        });

        // Assert
        result.AddedCount.Should().Be(1);
        result.AlreadyInPoolCount.Should().Be(1);
        result.NotFoundCount.Should().Be(1);
        _talentPoolCandidateRepositoryMock.Verify(r => r.AddAsync(It.Is<TalentPoolCandidate>(c => c.CandidateProfileId == 3)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoveCandidateAsync_WithUnknownMembership_ShouldThrowNotFoundException()
    {
        // Arrange
        _talentPoolCandidateRepositoryMock.Setup(r => r.GetByPoolAndCandidateAsync(1, 2))
            .ReturnsAsync((TalentPoolCandidate?)null);

        // Act
        var act = () => _service.RemoveCandidateAsync(1, 2);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task RemoveCandidateAsync_WithExistingMembership_ShouldDelete()
    {
        // Arrange
        var membership = new TalentPoolCandidate { TalentPoolId = 1, CandidateProfileId = 2 };
        _talentPoolCandidateRepositoryMock.Setup(r => r.GetByPoolAndCandidateAsync(1, 2)).ReturnsAsync(membership);

        // Act
        await _service.RemoveCandidateAsync(1, 2);

        // Assert
        _talentPoolCandidateRepositoryMock.Verify(r => r.Delete(membership), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        // Arrange
        _talentPoolRepositoryMock.Setup(r => r.GetByIdWithCandidatesAsync(99)).ReturnsAsync((TalentPool?)null);

        // Act
        var act = () => _service.GetByIdAsync(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task FastTrackAsync_WithUnknownJobPosting_ShouldThrowNotFoundException()
    {
        // Arrange
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(500)).ReturnsAsync((JobPosting?)null);

        // Act
        var act = () => _service.FastTrackAsync(new TalentPoolFastTrackRequest
        {
            CandidateProfileIds = new List<long> { 1 },
            JobPostingId = 500
        });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task FastTrackAsync_CreatesApplicationAndShortlistsUsingLatestResumeOnFile()
    {
        // Arrange
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new JobPosting { JobPostingId = 5 });

        var candidate = new CandidateProfile { CandidateProfileId = 1, FullName = "Alice Rahman", Email = "alice@example.com", Phone = "0123" };
        _candidateProfileRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(candidate);

        _jobApplicationRepositoryMock.Setup(r => r.GetByEmailAndJobPostingIdAsync("alice@example.com", 5))
            .ReturnsAsync((JobApplication?)null);

        var priorApplications = new List<JobApplication>
        {
            new() { JobApplicationId = 20, CandidateEmail = "alice@example.com", ResumeUrl = "/uploads/alice-cv.pdf" }
        };
        _jobApplicationRepositoryMock.Setup(r => r.GetByCandidateAsync(1, "alice@example.com"))
            .ReturnsAsync(priorApplications);

        _jobApplicationServiceMock.Setup(s => s.CreateAsync(It.IsAny<JobApplicationCreateRequest>(), It.IsAny<long?>())).ReturnsAsync(99);

        // Act
        var result = await _service.FastTrackAsync(new TalentPoolFastTrackRequest
        {
            CandidateProfileIds = new List<long> { 1 },
            JobPostingId = 5
        });

        // Assert
        result.ProcessedCount.Should().Be(1);
        result.FastTrackedCount.Should().Be(1);
        result.AlreadyAppliedCount.Should().Be(0);
        result.SkippedCount.Should().Be(0);

        _jobApplicationServiceMock.Verify(s => s.CreateAsync(It.Is<JobApplicationCreateRequest>(r =>
            r.JobPostingId == 5 && r.CandidateEmail == "alice@example.com" && r.ResumeUrl == "/uploads/alice-cv.pdf"), 1), Times.Once);
        _jobApplicationServiceMock.Verify(s => s.UpdateStatusAsync(99, It.Is<JobApplicationStatusUpdateRequest>(r =>
            r.ToStatus == ApplicationStatusEnum.Shortlisted)), Times.Once);
    }

    [Fact]
    public async Task FastTrackAsync_SkipsCandidateAlreadyAppliedToVacancy()
    {
        // Arrange
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new JobPosting { JobPostingId = 5 });

        var candidate = new CandidateProfile { CandidateProfileId = 1, FullName = "Alice Rahman", Email = "alice@example.com" };
        _candidateProfileRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(candidate);

        _jobApplicationRepositoryMock.Setup(r => r.GetByEmailAndJobPostingIdAsync("alice@example.com", 5))
            .ReturnsAsync(new JobApplication { JobApplicationId = 10, CandidateEmail = "alice@example.com", JobPostingId = 5 });

        // Act
        var result = await _service.FastTrackAsync(new TalentPoolFastTrackRequest
        {
            CandidateProfileIds = new List<long> { 1 },
            JobPostingId = 5
        });

        // Assert
        result.AlreadyAppliedCount.Should().Be(1);
        result.FastTrackedCount.Should().Be(0);
        _jobApplicationServiceMock.Verify(s => s.CreateAsync(It.IsAny<JobApplicationCreateRequest>()), Times.Never);
    }

    [Fact]
    public async Task FastTrackAsync_SkipsCandidateWithNoPriorApplicationToSourceResumeFrom()
    {
        // Arrange
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new JobPosting { JobPostingId = 5 });

        var candidate = new CandidateProfile { CandidateProfileId = 1, FullName = "Alice Rahman", Email = "alice@example.com" };
        _candidateProfileRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(candidate);

        _jobApplicationRepositoryMock.Setup(r => r.GetByEmailAndJobPostingIdAsync("alice@example.com", 5))
            .ReturnsAsync((JobApplication?)null);
        _jobApplicationRepositoryMock.Setup(r => r.GetByCandidateAsync(1, "alice@example.com"))
            .ReturnsAsync(new List<JobApplication>());

        // Act
        var result = await _service.FastTrackAsync(new TalentPoolFastTrackRequest
        {
            CandidateProfileIds = new List<long> { 1 },
            JobPostingId = 5
        });

        // Assert
        result.SkippedCount.Should().Be(1);
        result.FastTrackedCount.Should().Be(0);
        _jobApplicationServiceMock.Verify(s => s.CreateAsync(It.IsAny<JobApplicationCreateRequest>()), Times.Never);
    }
}
