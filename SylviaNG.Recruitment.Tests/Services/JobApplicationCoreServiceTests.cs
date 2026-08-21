using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

// A fresh file - the pre-split JobApplicationServiceTests.cs had zero coverage of CreateAsync/
// UpdateAsync/DeleteAsync/GetByIdAsync/GetPaginatedByJobPostingAsync (confirmed by grep before the
// split). Not attempting full coverage here - that gap is tracked separately by the testing audit
// - just enough smoke coverage so this new class isn't entirely untested.
public class JobApplicationCoreServiceTests
{
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<IJobPostingRepository> _jobPostingRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICandidateApplicationLinkResolver> _linkResolverMock;
    private readonly JobApplicationCoreService _service;

    public JobApplicationCoreServiceTests()
    {
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _jobPostingRepositoryMock = new Mock<IJobPostingRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _linkResolverMock = new Mock<ICandidateApplicationLinkResolver>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new JobApplicationCoreService(
            _jobApplicationRepositoryMock.Object,
            _jobPostingRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _linkResolverMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithOpenJobPosting_ShouldSaveAndReturnNewId()
    {
        var jobPosting = new JobPosting { JobPostingId = 1, CompanyId = 7 };
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(jobPosting);
        _jobApplicationRepositoryMock.Setup(r => r.GetByEmailAndJobPostingIdAsync("jane@example.com", 1)).ReturnsAsync((JobApplication?)null);
        _linkResolverMock.Setup(r => r.ResolveCandidateProfileIdAsync("jane@example.com")).ReturnsAsync(42);

        JobApplication? added = null;
        _jobApplicationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<JobApplication>()))
            .Callback<JobApplication>(a =>
            {
                a.JobApplicationId = 99;
                added = a;
            })
            .Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(new JobApplicationCreateRequest
        {
            JobPostingId = 1,
            CandidateName = "Jane Doe",
            CandidateEmail = "jane@example.com"
        });

        result.Should().Be(99);
        added.Should().NotBeNull();
        added!.CandidateProfileId.Should().Be(42);
        added.CompanyId.Should().Be(7);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithExplicitCandidateProfileId_ShouldBypassLinkResolver()
    {
        var jobPosting = new JobPosting { JobPostingId = 1, CompanyId = 7 };
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(jobPosting);
        _jobApplicationRepositoryMock.Setup(r => r.GetByEmailAndJobPostingIdAsync(It.IsAny<string>(), 1)).ReturnsAsync((JobApplication?)null);

        JobApplication? added = null;
        _jobApplicationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<JobApplication>()))
            .Callback<JobApplication>(a => added = a)
            .Returns(Task.CompletedTask);

        await _service.CreateAsync(new JobApplicationCreateRequest
        {
            JobPostingId = 1,
            CandidateName = "Jane Doe",
            CandidateEmail = "jane@example.com"
        }, candidateProfileId: 55);

        added!.CandidateProfileId.Should().Be(55);
        _linkResolverMock.Verify(r => r.ResolveCandidateProfileIdAsync(It.IsAny<string?>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateEmailForSameJobPosting_ShouldThrowDuplicateException()
    {
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new JobPosting { JobPostingId = 1 });
        _jobApplicationRepositoryMock.Setup(r => r.GetByEmailAndJobPostingIdAsync("jane@example.com", 1))
            .ReturnsAsync(new JobApplication { JobApplicationId = 5 });

        var act = () => _service.CreateAsync(new JobApplicationCreateRequest
        {
            JobPostingId = 1,
            CandidateName = "Jane Doe",
            CandidateEmail = "jane@example.com"
        });

        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async Task CreateAsync_WithUnknownJobPosting_ShouldThrowNotFoundException()
    {
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((JobPosting?)null);

        var act = () => _service.CreateAsync(new JobApplicationCreateRequest { JobPostingId = 1, CandidateName = "Jane Doe" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetByIdAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        _jobApplicationRepositoryMock
            .Setup(r => r.GetByIdWithIncludeAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<JobApplication, bool>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<JobApplication, object>>[]>()))
            .ReturnsAsync((JobApplication?)null);

        var act = () => _service.GetByIdAsync(1);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WithExistingApplication_ShouldDeleteAndSave()
    {
        var entity = new JobApplication { JobApplicationId = 1 };
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        await _service.DeleteAsync(1);

        _jobApplicationRepositoryMock.Verify(r => r.Delete(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
