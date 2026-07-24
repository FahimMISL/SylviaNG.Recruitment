using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class CandidateTagServiceTests
{
    private readonly Mock<ICandidateTagRepository> _candidateTagRepositoryMock;
    private readonly Mock<ICandidateProfileRepository> _candidateProfileRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CandidateTagService _service;

    public CandidateTagServiceTests()
    {
        _candidateTagRepositoryMock = new Mock<ICandidateTagRepository>();
        _candidateProfileRepositoryMock = new Mock<ICandidateProfileRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new CandidateTagService(
            _candidateTagRepositoryMock.Object,
            _candidateProfileRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedTags()
    {
        // Arrange
        var entities = new List<CandidateTag>
        {
            new() { CandidateTagId = 1, CandidateProfileId = 5, TagName = "Strong Communicator" },
            new() { CandidateTagId = 2, CandidateProfileId = 5, TagName = "Leadership Potential" },
        };
        _candidateTagRepositoryMock.Setup(r => r.GetAllByCandidateProfileIdAsync(5)).ReturnsAsync(entities);

        // Act
        var result = await _service.GetAllAsync(5);

        // Assert
        result.Should().HaveCount(2);
        result[0].TagName.Should().Be("Strong Communicator");
    }

    [Fact]
    public async Task CreateAsync_WithValidTag_ShouldAddAndSave()
    {
        // Arrange
        _candidateProfileRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new CandidateProfile { CandidateProfileId = 5 });
        _candidateTagRepositoryMock.Setup(r => r.GetAllByCandidateProfileIdAsync(5)).ReturnsAsync(new List<CandidateTag>());

        // Act
        await _service.CreateAsync(5, new CandidateTagCreateRequest { TagName = "  Fast Learner  " });

        // Assert
        _candidateTagRepositoryMock.Verify(r => r.AddAsync(It.Is<CandidateTag>(t => t.TagName == "Fast Learner" && t.CandidateProfileId == 5)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenProfileNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _candidateProfileRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((CandidateProfile?)null);

        // Act
        var act = () => _service.CreateAsync(999, new CandidateTagCreateRequest { TagName = "Fast Learner" });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateTagCaseInsensitive_ShouldThrowDuplicateException()
    {
        // Arrange
        _candidateProfileRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new CandidateProfile { CandidateProfileId = 5 });
        _candidateTagRepositoryMock.Setup(r => r.GetAllByCandidateProfileIdAsync(5))
            .ReturnsAsync(new List<CandidateTag> { new() { CandidateTagId = 1, CandidateProfileId = 5, TagName = "fast learner" } });

        // Act
        var act = () => _service.CreateAsync(5, new CandidateTagCreateRequest { TagName = "Fast Learner" });

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithValidTag_ShouldDelete()
    {
        // Arrange
        var entity = new CandidateTag { CandidateTagId = 1, CandidateProfileId = 5, TagName = "Fast Learner" };
        _candidateTagRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        await _service.DeleteAsync(5, 1);

        // Assert
        _candidateTagRepositoryMock.Verify(r => r.Delete(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenTagBelongsToDifferentProfile_ShouldThrowNotFoundException()
    {
        // Arrange
        var entity = new CandidateTag { CandidateTagId = 1, CandidateProfileId = 5, TagName = "Fast Learner" };
        _candidateTagRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        var act = () => _service.DeleteAsync(999, 1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetSuggestionsAsync_ShouldDelegateToRepository()
    {
        // Arrange
        _candidateTagRepositoryMock.Setup(r => r.GetDistinctTagNamesAsync("lead", 20))
            .ReturnsAsync(new List<string> { "Leadership Potential" });

        // Act
        var result = await _service.GetSuggestionsAsync("lead");

        // Assert
        result.Should().ContainSingle().Which.Should().Be("Leadership Potential");
    }
}
