using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationEvents.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationEvents.Commands.NotificationEventCreate
{
    public class NotificationEventCreateCommand : IRequest<long>
    {
        public NotificationEventCreateRequest Request { get; set; }

        public NotificationEventCreateCommand(NotificationEventCreateRequest request)
        {
            Request = request;
        }
    }
}
