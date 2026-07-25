using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class NotificationTemplateServiceTests
{
    private readonly Mock<INotificationTemplateRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly NotificationTemplateService _service;

    public NotificationTemplateServiceTests()
    {
        _repositoryMock = new Mock<INotificationTemplateRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new NotificationTemplateService(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_DuplicateCode_ShouldThrowDuplicateException()
    {
        _repositoryMock.Setup(r => r.ExistsByCodeAsync("ADMIT_CARD_ISSUED_EMAIL", null)).ReturnsAsync(true);

        var act = () => _service.CreateAsync(new NotificationTemplateCreateRequest
        {
            Channel = NotificationChannelEnum.Email,
            Code = "ADMIT_CARD_ISSUED_EMAIL",
            Name = "Admit Card Issued",
            Subject = "Your admit card",
            Body = "Hi {{CandidateName}}",
        });

        await act.Should().ThrowAsync<DuplicateException>();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<NotificationTemplate>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ShouldAddTemplateAndFirstVersion()
    {
        _repositoryMock.Setup(r => r.ExistsByCodeAsync("ADMIT_CARD_ISSUED_EMAIL", null)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<NotificationTemplate>()))
            .Callback<NotificationTemplate>(e => e.NotificationTemplateId = 7)
            .Returns(Task.CompletedTask);

        var id = await _service.CreateAsync(new NotificationTemplateCreateRequest
        {
            Channel = NotificationChannelEnum.Email,
            Code = "ADMIT_CARD_ISSUED_EMAIL",
            Name = "Admit Card Issued",
            Subject = "Your admit card",
            Body = "Hi {{CandidateName}}",
        });

        id.Should().Be(7);
        _repositoryMock.Verify(r => r.AddVersionAsync(It.Is<NotificationTemplateVersion>(v =>
            v.NotificationTemplateId == 7 && v.VersionNumber == 1 && v.Body == "Hi {{CandidateName}}")), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task UpdateAsync_UnknownId_ShouldThrowNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((NotificationTemplate?)null);

        var act = () => _service.UpdateAsync(99, new NotificationTemplateUpdateRequest { Name = "X", Body = "Y" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_ShouldBumpVersionAndInsertSnapshot()
    {
        var entity = new NotificationTemplate
        {
            NotificationTemplateId = 1,
            Channel = NotificationChannelEnum.Email,
            Code = "ADMIT_CARD_ISSUED_EMAIL",
            Name = "Old Name",
            Body = "Old body",
            CurrentVersionNumber = 1,
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        await _service.UpdateAsync(1, new NotificationTemplateUpdateRequest
        {
            Name = "New Name",
            Subject = "New Subject",
            Body = "New body {{CandidateName}}",
            IsActive = true,
        });

        entity.CurrentVersionNumber.Should().Be(2);
        entity.Name.Should().Be("New Name");
        _repositoryMock.Verify(r => r.AddVersionAsync(It.Is<NotificationTemplateVersion>(v =>
            v.NotificationTemplateId == 1 && v.VersionNumber == 2 && v.Body == "New body {{CandidateName}}")), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_InUse_ShouldThrowResourceInUseException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new NotificationTemplate { NotificationTemplateId = 1 });
        _repositoryMock.Setup(r => r.CountMappingUsageAsync(1)).ReturnsAsync(2);

        var act = () => _service.DeleteAsync(1);

        await act.Should().ThrowAsync<ResourceInUseException>();
        _repositoryMock.Verify(r => r.Delete(It.IsAny<NotificationTemplate>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_NotInUse_ShouldDelete()
    {
        var entity = new NotificationTemplate { NotificationTemplateId = 1 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.CountMappingUsageAsync(1)).ReturnsAsync(0);

        await _service.DeleteAsync(1);

        _repositoryMock.Verify(r => r.Delete(entity), Times.Once);
    }

    [Fact]
    public async Task GetVersionsAsync_UnknownId_ShouldThrowNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((NotificationTemplate?)null);

        var act = () => _service.GetVersionsAsync(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetVersionsAsync_KnownId_ShouldReturnOrderedVersions()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new NotificationTemplate { NotificationTemplateId = 1 });
        _repositoryMock.Setup(r => r.GetVersionsOrderedAsync(1)).ReturnsAsync(new List<NotificationTemplateVersion>
        {
            new() { NotificationTemplateVersionId = 2, NotificationTemplateId = 1, VersionNumber = 2, Body = "v2" },
            new() { NotificationTemplateVersionId = 1, NotificationTemplateId = 1, VersionNumber = 1, Body = "v1" },
        });

        var result = await _service.GetVersionsAsync(1);

        result.Should().HaveCount(2);
        result[0].VersionNumber.Should().Be(2);
    }
}
