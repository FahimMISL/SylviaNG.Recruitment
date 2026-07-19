using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using System.Linq.Expressions;

namespace SylviaNG.Recruitment.Tests.Services;

public class CandidateProfileServiceTests
{
    private readonly Mock<ICandidateProfileRepository> _candidateProfileRepositoryMock;
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<ITalentPoolCandidateRepository> _talentPoolCandidateRepositoryMock;
    private readonly Mock<ICurrentCandidateService> _currentCandidateServiceMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CandidateProfileService _service;

    public CandidateProfileServiceTests()
    {
        _candidateProfileRepositoryMock = new Mock<ICandidateProfileRepository>();
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _talentPoolCandidateRepositoryMock = new Mock<ITalentPoolCandidateRepository>();
        _currentCandidateServiceMock = new Mock<ICurrentCandidateService>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _talentPoolCandidateRepositoryMock.Setup(r => r.GetAllByCandidateProfileIdAsync(It.IsAny<long>()))
            .ReturnsAsync(new List<TalentPoolCandidate>());

        _service = new CandidateProfileService(
            _candidateProfileRepositoryMock.Object,
            _jobApplicationRepositoryMock.Object,
            _talentPoolCandidateRepositoryMock.Object,
            _currentCandidateServiceMock.Object,
            _fileStorageServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnMappedSummaries()
    {
        // Arrange
        var entities = new List<CandidateProfile>
        {
            new() { CandidateProfileId = 1, FullName = "Alice Rahman", Email = "alice@example.com" },
            new() { CandidateProfileId = 2, FullName = "Bilal Hossain", Email = "bilal@example.com" },
        };

        var request = new PagedRequest { Page = 1, PageSize = 10 };
        _candidateProfileRepositoryMock.Setup(r => r.GetPagedAsync(request))
            .ReturnsAsync(new PagedResult<CandidateProfile> { Data = entities, TotalCount = 2, PageNumber = 1, PageSize = 10 });

        // Act
        var result = await _service.GetPagedAsync(request);

        // Assert
        result.TotalCount.Should().Be(2);
        result.Data.Should().HaveCount(2);
        result.Data[0].FullName.Should().Be("Alice Rahman");
        result.Data[1].Email.Should().Be("bilal@example.com");
    }

    [Fact]
    public async Task GetProfileDetailAsync_WithExistingId_ShouldReturnDetailWithApplicationHistory()
    {
        // Arrange
        var entity = new CandidateProfile
        {
            CandidateProfileId = 1,
            FullName = "Alice Rahman",
            Email = "alice@example.com",
            HrNotes = "Strong candidate",
            Educations = new List<CandidateEducation>(),
            WorkExperiences = new List<CandidateWorkExperience>(),
            Skills = new List<CandidateSkill>(),
            Certifications = new List<CandidateCertification>(),
            Documents = new List<CandidateDocument>(),
        };

        _candidateProfileRepositoryMock.Setup(r => r.GetByIdWithIncludeAsync(
            It.IsAny<Expression<Func<CandidateProfile, bool>>>(),
            It.IsAny<Expression<Func<CandidateProfile, object>>[]>()))
            .ReturnsAsync(entity);

        var applications = new List<JobApplication>
        {
            new()
            {
                JobApplicationId = 10,
                JobPostingId = 5,
                CandidateEmail = "alice@example.com",
                CandidateName = "Alice Rahman",
                ApplicationStatus = ApplicationStatusEnum.Shortlisted,
                JobPosting = new JobPosting { JobPostingId = 5, Title = "Backend Engineer" },
            },
        };
        _jobApplicationRepositoryMock.Setup(r => r.GetByCandidateEmailAsync("alice@example.com"))
            .ReturnsAsync(applications);

        // Act
        var result = await _service.GetProfileDetailAsync(1);

        // Assert
        result.CandidateProfileId.Should().Be(1);
        result.HrNotes.Should().Be("Strong candidate");
        result.ApplicationHistory.Should().HaveCount(1);
        result.ApplicationHistory[0].JobPostingTitle.Should().Be("Backend Engineer");
        result.ApplicationHistory[0].ApplicationStatus.Should().Be(ApplicationStatusEnum.Shortlisted);
    }

    [Fact]
    public async Task GetProfileDetailAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        _candidateProfileRepositoryMock.Setup(r => r.GetByIdWithIncludeAsync(
            It.IsAny<Expression<Func<CandidateProfile, bool>>>(),
            It.IsAny<Expression<Func<CandidateProfile, object>>[]>()))
            .ReturnsAsync((CandidateProfile?)null);

        // Act
        var act = () => _service.GetProfileDetailAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateHrNotesAsync_WithExistingId_ShouldUpdateAndSave()
    {
        // Arrange
        var entity = new CandidateProfile { CandidateProfileId = 1, FullName = "Alice Rahman", Email = "alice@example.com" };
        _candidateProfileRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        await _service.UpdateHrNotesAsync(1, "Interviewed on 2026-07-10, strong communicator.");

        // Assert
        entity.HrNotes.Should().Be("Interviewed on 2026-07-10, strong communicator.");
        entity.UpdatedAt.Should().NotBeNull();
        _candidateProfileRepositoryMock.Verify(r => r.Update(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateHrNotesAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        _candidateProfileRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((CandidateProfile?)null);

        // Act
        var act = () => _service.UpdateHrNotesAsync(999, "notes");

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
