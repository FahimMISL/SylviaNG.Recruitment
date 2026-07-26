using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogExportExcel
{
    public class NotificationLogExportExcelQuery : IRequest<NotificationLogFileResponse>
    {
        public NotificationLogFilterRequest Filter { get; }

        public NotificationLogExportExcelQuery(NotificationLogFilterRequest filter)
        {
            Filter = filter;
        }
    }
}
