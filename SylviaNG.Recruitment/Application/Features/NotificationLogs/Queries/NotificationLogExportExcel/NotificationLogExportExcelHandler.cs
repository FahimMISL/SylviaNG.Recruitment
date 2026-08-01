using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogExportExcel
{
    public class NotificationLogExportExcelHandler : IRequestHandler<NotificationLogExportExcelQuery, NotificationLogFileResponse>
    {
        private readonly INotificationLogService _notificationLogService;

        public NotificationLogExportExcelHandler(INotificationLogService notificationLogService)
        {
            _notificationLogService = notificationLogService;
        }

        public async Task<NotificationLogFileResponse> Handle(NotificationLogExportExcelQuery query, CancellationToken cancellationToken)
        {
            return await _notificationLogService.ExportExcelAsync(query.Filter);
        }
    }
}
