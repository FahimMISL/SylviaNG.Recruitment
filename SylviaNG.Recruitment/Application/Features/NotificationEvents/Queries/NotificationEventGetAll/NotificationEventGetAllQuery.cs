using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationEvents.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationEvents.Queries.NotificationEventGetAll
{
    public class NotificationEventGetAllQuery : IRequest<List<NotificationEventResponse>>
    {
    }
}
