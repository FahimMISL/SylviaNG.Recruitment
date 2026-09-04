using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class ProfileFieldConfigServiceTests
{
    private readonly Mock<IProfileFieldConfigRepository> _repositoryMock;
    private readonly Mock<IJobPostingRepository> _jobPostingRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ProfileFieldConfigService _service;

    public ProfileFieldConfigServiceTests()
    {
        _repositoryMock = new Mock<IProfileFieldConfigRepository>();
        _jobPostingRepositoryMock = new Mock<IJobPostingRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new ProfileFieldConfigService(_repositoryMock.Object, _jobPostingRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetEffectiveConfigAsync_WithNoConfiguredRows_ShouldDefaultEveryFieldToOptional()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetGlobalAndForJobPostingAsync(It.IsAny<long?>()))
            .ReturnsAsync(new List<ProfileFieldConfig>());

        // Act
        var result = await _service.GetEffectiveConfigAsync(jobPostingId: null);

        // Assert
        result.Should().HaveCount(Enum.GetValues<CandidateProfileFieldEnum>().Length);
        result.Should().OnlyContain(r => r.Visibility == ProfileFieldVisibilityEnum.Optional);
    }

    [Fact]
    public async Task GetEffectiveConfigAsync_WithJobPostingOverride_ShouldWinOverGlobalDefault()
    {
        // Arrange: global says Hidden, this specific posting overrides to Mandatory.
        var rows = new List<ProfileFieldConfig>
        {
            new() { Field = CandidateProfileFieldEnum.PhoneNumber, JobPostingId = null, Visibility = ProfileFieldVisibilityEnum.Hidden },
            new() { Field = CandidateProfileFieldEnum.PhoneNumber, JobPostingId = 5, Visibility = ProfileFieldVisibilityEnum.Mandatory },
        };
        _repositoryMock.Setup(r => r.GetGlobalAndForJobPostingAsync(5)).ReturnsAsync(rows);

        // Act
        var result = await _service.GetEffectiveConfigAsync(jobPostingId: 5);

        // Assert
        var phoneNumber = result.Single(r => r.Field == CandidateProfileFieldEnum.PhoneNumber);
        phoneNumber.Visibility.Should().Be(ProfileFieldVisibilityEnum.Mandatory);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateScope_ShouldThrowDuplicateException()
    {
        // Arrange
        var request = new ProfileFieldConfigRequest { Field = CandidateProfileFieldEnum.Gender, JobPostingId = null, Visibility = ProfileFieldVisibilityEnum.Hidden };
        _repositoryMock.Setup(r => r.ExistsAsync(request.Field, request.JobPostingId, null)).ReturnsAsync(true);

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentJobPosting_ShouldThrowNotFoundException()
    {
        // Arrange
        var request = new ProfileFieldConfigRequest { Field = CandidateProfileFieldEnum.Gender, JobPostingId = 999, Visibility = ProfileFieldVisibilityEnum.Hidden };
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((JobPosting?)null);

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
