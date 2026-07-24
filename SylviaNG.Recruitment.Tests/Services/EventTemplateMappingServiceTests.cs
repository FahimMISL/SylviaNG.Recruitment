using FluentAssertions;
using FluentValidation;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class EventTemplateMappingServiceTests
{
    private readonly Mock<IEventTemplateMappingRepository> _mappingRepositoryMock;
    private readonly Mock<INotificationTemplateRepository> _templateRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly EventTemplateMappingService _service;

    public EventTemplateMappingServiceTests()
    {
        _mappingRepositoryMock = new Mock<IEventTemplateMappingRepository>();
        _templateRepositoryMock = new Mock<INotificationTemplateRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new EventTemplateMappingService(_mappingRepositoryMock.Object, _templateRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    private static NotificationTemplate EmailTemplate(long id = 1) => new()
    {
        NotificationTemplateId = id,
        Channel = NotificationChannelEnum.Email,
        Name = "Admit Card Issued",
    };

    [Fact]
    public async Task CreateAsync_TemplateChannelMismatch_ShouldThrowValidationException()
    {
        _templateRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(EmailTemplate());

        var act = () => _service.CreateAsync(new EventTemplateMappingCreateRequest
        {
            RecruitmentEvent = RecruitmentEventEnum.AdmitCardIssued,
            Channel = NotificationChannelEnum.Sms,
            RecipientType = NotificationRecipientTypeEnum.Candidate,
            NotificationTemplateId = 1,
        });

        await act.Should().ThrowAsync<ValidationException>();
        _mappingRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EventTemplateMapping>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_DuplicateKey_ShouldThrowDuplicateException()
    {
        _templateRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(EmailTemplate());
        _mappingRepositoryMock.Setup(r => r.ExistsAsync(
            RecruitmentEventEnum.AdmitCardIssued, NotificationChannelEnum.Email, NotificationRecipientTypeEnum.Candidate, null))
            .ReturnsAsync(true);

        var act = () => _service.CreateAsync(new EventTemplateMappingCreateRequest
        {
            RecruitmentEvent = RecruitmentEventEnum.AdmitCardIssued,
            Channel = NotificationChannelEnum.Email,
            RecipientType = NotificationRecipientTypeEnum.Candidate,
            NotificationTemplateId = 1,
        });

        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ShouldAddAndReturnId()
    {
        _templateRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(EmailTemplate());
        _mappingRepositoryMock.Setup(r => r.ExistsAsync(
            RecruitmentEventEnum.AdmitCardIssued, NotificationChannelEnum.Email, NotificationRecipientTypeEnum.Candidate, null))
            .ReturnsAsync(false);
        _mappingRepositoryMock.Setup(r => r.AddAsync(It.IsAny<EventTemplateMapping>()))
            .Callback<EventTemplateMapping>(e => e.EventTemplateMappingId = 3)
            .Returns(Task.CompletedTask);

        var id = await _service.CreateAsync(new EventTemplateMappingCreateRequest
        {
            RecruitmentEvent = RecruitmentEventEnum.AdmitCardIssued,
            Channel = NotificationChannelEnum.Email,
            RecipientType = NotificationRecipientTypeEnum.Candidate,
            NotificationTemplateId = 1,
        });

        id.Should().Be(3);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_UnknownId_ShouldThrowNotFoundException()
    {
        _mappingRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((EventTemplateMapping?)null);

        var act = () => _service.DeleteAsync(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedResponses()
    {
        _mappingRepositoryMock.Setup(r => r.GetAllWithTemplateAsync()).ReturnsAsync(new List<EventTemplateMapping>
        {
            new()
            {
                EventTemplateMappingId = 1,
                RecruitmentEvent = RecruitmentEventEnum.AdmitCardIssued,
                Channel = NotificationChannelEnum.Email,
                RecipientType = NotificationRecipientTypeEnum.Candidate,
                NotificationTemplateId = 1,
                NotificationTemplate = EmailTemplate(),
            },
        });

        var result = await _service.GetAllAsync();

        result.Should().ContainSingle(r => r.NotificationTemplateName == "Admit Card Issued");
    }
}
