using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class MajorSubjectUniversityServiceTests
{
    private readonly Mock<IMajorSubjectUniversityRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly MajorSubjectUniversityService _service;

    public MajorSubjectUniversityServiceTests()
    {
        _repositoryMock = new Mock<IMajorSubjectUniversityRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new MajorSubjectUniversityService(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_DuplicateName_ShouldThrowDuplicateException()
    {
        _repositoryMock.Setup(r => r.ExistsByNameAsync("Economics", null)).ReturnsAsync(true);

        var act = () => _service.CreateAsync(new MajorSubjectUniversityCreateRequest { Name = "Economics" });

        await act.Should().ThrowAsync<DuplicateException>();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<MajorSubjectUniversity>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ValidName_ShouldAddAndReturnId()
    {
        _repositoryMock.Setup(r => r.ExistsByNameAsync("Economics", null)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<MajorSubjectUniversity>()))
            .Callback<MajorSubjectUniversity>(e => e.MajorSubjectUniversityId = 7)
            .Returns(Task.CompletedTask);

        var id = await _service.CreateAsync(new MajorSubjectUniversityCreateRequest { Name = "Economics" });

        id.Should().Be(7);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UnknownId_ShouldThrowNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((MajorSubjectUniversity?)null);

        var act = () => _service.UpdateAsync(99, new MajorSubjectUniversityUpdateRequest { Name = "Economics" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_NameTakenByAnotherRow_ShouldThrowDuplicateException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new MajorSubjectUniversity { MajorSubjectUniversityId = 1, Name = "Finance" });
        _repositoryMock.Setup(r => r.ExistsByNameAsync("Economics", 1)).ReturnsAsync(true);

        var act = () => _service.UpdateAsync(1, new MajorSubjectUniversityUpdateRequest { Name = "Economics" });

        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async Task DeleteAsync_InUse_ShouldThrowResourceInUseException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new MajorSubjectUniversity { MajorSubjectUniversityId = 1, Name = "Economics" });
        _repositoryMock.Setup(r => r.CountUsageAsync(1)).ReturnsAsync(2);

        var act = () => _service.DeleteAsync(1);

        await act.Should().ThrowAsync<ResourceInUseException>();
        _repositoryMock.Verify(r => r.Delete(It.IsAny<MajorSubjectUniversity>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_NotInUse_ShouldDelete()
    {
        var entity = new MajorSubjectUniversity { MajorSubjectUniversityId = 1, Name = "Economics" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.CountUsageAsync(1)).ReturnsAsync(0);

        await _service.DeleteAsync(1);

        _repositoryMock.Verify(r => r.Delete(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedResponses()
    {
        _repositoryMock.Setup(r => r.GetAllOrderedAsync()).ReturnsAsync(new List<MajorSubjectUniversity>
        {
            new() { MajorSubjectUniversityId = 1, Name = "Accounting" },
            new() { MajorSubjectUniversityId = 2, Name = "Economics" },
        });

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().Contain(r => r.Name == "Economics");
    }
}
