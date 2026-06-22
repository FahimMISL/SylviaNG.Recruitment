using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Kafka;

namespace SylviaNG.Recruitment.Application.Features.Requisitions.Commands.RequisitionUpdate
{
    public class RequisitionUpdateHandler : IRequestHandler<RequisitionUpdateCommand, Unit>
    {
        private readonly IRequisitionService _service;
        private readonly IRecruitmentEventProducer _eventProducer;
        private readonly IUserNotificationService _notificationService;

        public RequisitionUpdateHandler(IRequisitionService service, IRecruitmentEventProducer eventProducer, IUserNotificationService notificationService)
        {
            _service = service;
            _eventProducer = eventProducer;
            _notificationService = notificationService;
        }

        public async Task<Unit> Handle(RequisitionUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.RequisitionId, command.Request);

            var title = command.Request.Title ?? "";

            switch (command.Request.RequisitionStatus)
            {
                case RequisitionStatusEnum.Submitted:
                    await _eventProducer.PublishAsync(NotificationEventConsumer.TOPIC_REQUISITION, new { action = "SUBMITTED", requisitionId = command.RequisitionId, title });
                    await _notificationService.NotifyRoleAsync("Admin", "Requisition Pending Approval", $"\"{title}\" needs your review.", UserNotificationTypeEnum.Warning, "/requisitions");
                    break;
                case RequisitionStatusEnum.Approved:
                    await _eventProducer.PublishAsync(NotificationEventConsumer.TOPIC_REQUISITION, new { action = "APPROVED", requisitionId = command.RequisitionId, title });
                    await _notificationService.NotifyRoleAsync("HR", "Requisition Approved", $"\"{title}\" has been approved.", UserNotificationTypeEnum.Success, "/requisitions");
                    break;
                case RequisitionStatusEnum.Rejected:
                    await _eventProducer.PublishAsync(NotificationEventConsumer.TOPIC_REQUISITION, new { action = "REJECTED", requisitionId = command.RequisitionId, title });
                    await _notificationService.NotifyRoleAsync("HR", "Requisition Rejected", $"\"{title}\" has been rejected.", UserNotificationTypeEnum.Error, "/requisitions");
                    break;
            }

            return Unit.Value;
        }
    }
}
