using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.CandidateRecommendations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class CandidateRecommendationServiceTests
{
    private readonly Mock<ICandidateRecommendationRepository> _candidateRecommendationRepositoryMock;
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CandidateRecommendationService _service;

    public CandidateRecommendationServiceTests()
    {
        _candidateRecommendationRepositoryMock = new Mock<ICandidateRecommendationRepository>();
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _currentUserServiceMock.Setup(c => c.GetCurrentUserName()).Returns("abir");

        _service = new CandidateRecommendationService(
            _candidateRecommendationRepositoryMock.Object,
            _jobApplicationRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    private static CandidateRecommendationCreateRequest CreateRequest(string justification = "Exceptional interview performance, strong references.")
    {
        return new CandidateRecommendationCreateRequest { Justification = justification };
    }

    [Fact]
    public async Task CreateAsync_WithUnknownJobApplicationId_ShouldThrowNotFoundException()
    {
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((JobApplication?)null);

        var act = () => _service.CreateAsync(99, CreateRequest());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_WithBlankJustification_ShouldThrowValidationException()
    {
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new JobApplication { JobApplicationId = 1 });

        var act = () => _service.CreateAsync(1, CreateRequest("   "));

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        _candidateRecommendationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CandidateRecommendation>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithExistingPendingRecommendation_ShouldThrowValidationException()
    {
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new JobApplication { JobApplicationId = 1 });
        _candidateRecommendationRepositoryMock.Setup(r => r.GetLatestByJobApplicationIdAsync(1))
            .ReturnsAsync(new CandidateRecommendation { JobApplicationId = 1, Status = RecommendationStatusEnum.Pending });

        var act = () => _service.CreateAsync(1, CreateRequest());

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async Task CreateAsync_WithPriorReviewedRecommendation_ShouldAllowNewOne()
    {
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new JobApplication { JobApplicationId = 1 });
        _candidateRecommendationRepositoryMock.Setup(r => r.GetLatestByJobApplicationIdAsync(1))
            .ReturnsAsync(new CandidateRecommendation { JobApplicationId = 1, Status = RecommendationStatusEnum.Rejected });

        CandidateRecommendation? saved = null;
        _candidateRecommendationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<CandidateRecommendation>()))
            .Callback<CandidateRecommendation>(r => { r.CandidateRecommendationId = 5; saved = r; })
            .Returns(Task.CompletedTask);

        var id = await _service.CreateAsync(1, CreateRequest());

        id.Should().Be(5);
        saved!.RecommendedByUserName.Should().Be("abir");
        saved.Status.Should().Be(RecommendationStatusEnum.Pending);
    }

    [Fact]
    public async Task CreateAsync_WithNoResolvableCurrentUser_ShouldThrowValidationException()
    {
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new JobApplication { JobApplicationId = 1 });
        _candidateRecommendationRepositoryMock.Setup(r => r.GetLatestByJobApplicationIdAsync(1)).ReturnsAsync((CandidateRecommendation?)null);
        _currentUserServiceMock.Setup(c => c.GetCurrentUserName()).Returns((string?)null);

        var act = () => _service.CreateAsync(1, CreateRequest());

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        _candidateRecommendationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CandidateRecommendation>()), Times.Never);
    }

    [Fact]
    public async Task GetLatestByJobApplicationIdAsync_WithNoRecommendation_ShouldReturnNull()
    {
        _candidateRecommendationRepositoryMock.Setup(r => r.GetLatestByJobApplicationIdAsync(1)).ReturnsAsync((CandidateRecommendation?)null);

        var result = await _service.GetLatestByJobApplicationIdAsync(1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ReviewAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        _candidateRecommendationRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((CandidateRecommendation?)null);

        var act = () => _service.ReviewAsync(99, new CandidateRecommendationReviewRequest { Status = RecommendationStatusEnum.Accepted });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ReviewAsync_AlreadyReviewed_ShouldThrowValidationException()
    {
        var entity = new CandidateRecommendation { CandidateRecommendationId = 1, Status = RecommendationStatusEnum.Accepted };
        _candidateRecommendationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var act = () => _service.ReviewAsync(1, new CandidateRecommendationReviewRequest { Status = RecommendationStatusEnum.Rejected });

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async Task ReviewAsync_WithPendingStatusInRequest_ShouldThrowValidationException()
    {
        var entity = new CandidateRecommendation { CandidateRecommendationId = 1, Status = RecommendationStatusEnum.Pending };
        _candidateRecommendationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var act = () => _service.ReviewAsync(1, new CandidateRecommendationReviewRequest { Status = RecommendationStatusEnum.Pending });

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async Task ReviewAsync_Accept_ShouldSetStatusAndReviewer()
    {
        var entity = new CandidateRecommendation { CandidateRecommendationId = 1, Status = RecommendationStatusEnum.Pending };
        _candidateRecommendationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _currentUserServiceMock.Setup(c => c.GetCurrentUserName()).Returns("admin");

        await _service.ReviewAsync(1, new CandidateRecommendationReviewRequest { Status = RecommendationStatusEnum.Accepted, ReviewComments = "Great fit." });

        entity.Status.Should().Be(RecommendationStatusEnum.Accepted);
        entity.ReviewComments.Should().Be("Great fit.");
        entity.ReviewedByUserName.Should().Be("admin");
        entity.ReviewedAt.Should().NotBeNull();
        _candidateRecommendationRepositoryMock.Verify(r => r.Update(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPendingAsync_ShouldMapToListItems()
    {
        var application = new JobApplication { JobApplicationId = 1, CandidateName = "Sadia", JobPosting = new Domain.Entities.JobPosting { Title = "Backend Engineer" } };
        var entity = new CandidateRecommendation
        {
            CandidateRecommendationId = 1,
            JobApplicationId = 1,
            Justification = "Strong candidate",
            RecommendedByUserName = "abir",
            RecommendedAt = DateTime.UtcNow,
            JobApplication = application,
        };
        _candidateRecommendationRepositoryMock.Setup(r => r.GetPendingWithApplicationAsync()).ReturnsAsync(new List<CandidateRecommendation> { entity });

        var result = await _service.GetPendingAsync();

        result.Should().ContainSingle();
        result[0].CandidateName.Should().Be("Sadia");
        result[0].JobPostingTitle.Should().Be("Backend Engineer");
    }
}
