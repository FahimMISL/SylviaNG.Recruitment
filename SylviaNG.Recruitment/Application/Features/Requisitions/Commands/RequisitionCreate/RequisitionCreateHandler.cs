using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Kafka;

namespace SylviaNG.Recruitment.Application.Features.Requisitions.Commands.RequisitionCreate
{
    public class RequisitionCreateHandler : IRequestHandler<RequisitionCreateCommand, long>
    {
        private readonly IRequisitionService _service;
        private readonly IRecruitmentEventProducer _eventProducer;
        private readonly IUserNotificationService _notificationService;

        public RequisitionCreateHandler(IRequisitionService service, IRecruitmentEventProducer eventProducer, IUserNotificationService notificationService)
        {
            _service = service;
            _eventProducer = eventProducer;
            _notificationService = notificationService;
        }

        public async Task<long> Handle(RequisitionCreateCommand command, CancellationToken cancellationToken)
        {
            var id = await _service.CreateAsync(command.Request);

            if (command.Request.RequisitionStatus == RequisitionStatusEnum.Submitted)
            {
                await _eventProducer.PublishAsync(NotificationEventConsumer.TOPIC_REQUISITION, new { action = "SUBMITTED", requisitionId = id, title = command.Request.Title });
                await _notificationService.NotifyRoleAsync("Admin", "Requisition Pending Approval", $"\"{command.Request.Title}\" needs your review.", UserNotificationTypeEnum.Warning, "/requisitions");
            }

            return id;
        }
    }
}
