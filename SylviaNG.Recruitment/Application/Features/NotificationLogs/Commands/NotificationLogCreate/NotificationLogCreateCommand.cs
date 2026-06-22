using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogCreate
{
    public class NotificationLogCreateCommand : IRequest<long>
    {
        public NotificationLogCreateRequest Request { get; set; }

        public NotificationLogCreateCommand(NotificationLogCreateRequest request)
        {
            Request = request;
        }
    }
}
