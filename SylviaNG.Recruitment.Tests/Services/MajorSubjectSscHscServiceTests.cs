using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class MajorSubjectSscHscServiceTests
{
    private readonly Mock<IMajorSubjectSscHscRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly MajorSubjectSscHscService _service;

    public MajorSubjectSscHscServiceTests()
    {
        _repositoryMock = new Mock<IMajorSubjectSscHscRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new MajorSubjectSscHscService(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_DuplicateName_ShouldThrowDuplicateException()
    {
        _repositoryMock.Setup(r => r.ExistsByNameAsync("Science", null)).ReturnsAsync(true);

        var act = () => _service.CreateAsync(new MajorSubjectSscHscCreateRequest { Name = "Science" });

        await act.Should().ThrowAsync<DuplicateException>();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<MajorSubjectSscHsc>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ValidName_ShouldAddAndReturnId()
    {
        _repositoryMock.Setup(r => r.ExistsByNameAsync("Science", null)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<MajorSubjectSscHsc>()))
            .Callback<MajorSubjectSscHsc>(e => e.MajorSubjectSscHscId = 5)
            .Returns(Task.CompletedTask);

        var id = await _service.CreateAsync(new MajorSubjectSscHscCreateRequest { Name = "Science" });

        id.Should().Be(5);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UnknownId_ShouldThrowNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((MajorSubjectSscHsc?)null);

        var act = () => _service.UpdateAsync(99, new MajorSubjectSscHscUpdateRequest { Name = "Science" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_NameTakenByAnotherRow_ShouldThrowDuplicateException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new MajorSubjectSscHsc { MajorSubjectSscHscId = 1, Name = "Arts" });
        _repositoryMock.Setup(r => r.ExistsByNameAsync("Science", 1)).ReturnsAsync(true);

        var act = () => _service.UpdateAsync(1, new MajorSubjectSscHscUpdateRequest { Name = "Science" });

        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async Task DeleteAsync_InUse_ShouldThrowResourceInUseException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new MajorSubjectSscHsc { MajorSubjectSscHscId = 1, Name = "Science" });
        _repositoryMock.Setup(r => r.CountUsageAsync(1)).ReturnsAsync(3);

        var act = () => _service.DeleteAsync(1);

        await act.Should().ThrowAsync<ResourceInUseException>();
        _repositoryMock.Verify(r => r.Delete(It.IsAny<MajorSubjectSscHsc>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_NotInUse_ShouldDelete()
    {
        var entity = new MajorSubjectSscHsc { MajorSubjectSscHscId = 1, Name = "Science" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.CountUsageAsync(1)).ReturnsAsync(0);

        await _service.DeleteAsync(1);

        _repositoryMock.Verify(r => r.Delete(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedResponses()
    {
        _repositoryMock.Setup(r => r.GetAllOrderedAsync()).ReturnsAsync(new List<MajorSubjectSscHsc>
        {
            new() { MajorSubjectSscHscId = 1, Name = "Arts" },
            new() { MajorSubjectSscHscId = 2, Name = "Science" },
        });

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().Contain(r => r.Name == "Science");
    }
}
