using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.BulkNotificationSend
{
    public class BulkNotificationSendHandler : IRequestHandler<BulkNotificationSendCommand, BulkNotificationResult>
    {
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public BulkNotificationSendHandler(IEmailService emailService, ISmsService smsService)
        {
            _emailService = emailService;
            _smsService = smsService;
        }

        public async Task<BulkNotificationResult> Handle(BulkNotificationSendCommand command, CancellationToken cancellationToken)
        {
            var req = command.Request;
            var result = new BulkNotificationResult();

            if (req.Channel == NotificationChannelEnum.SMS)
            {
                var smsResults = await _smsService.SendBulkAsync(req.Recipients, req.Message);
                result.TotalSent = smsResults.Count(r => r.Success);
                result.TotalFailed = smsResults.Count(r => !r.Success);
                result.FailedRecipients = smsResults.Where(r => !r.Success).Select(r => r.PhoneNumber).ToList();
            }
            else if (req.Channel == NotificationChannelEnum.Email)
            {
                foreach (var recipient in req.Recipients)
                {
                    try
                    {
                        await _emailService.SendAsync(recipient, recipient, req.Subject, req.Message);
                        result.TotalSent++;
                    }
                    catch
                    {
                        result.TotalFailed++;
                        result.FailedRecipients.Add(recipient);
                    }
                }
            }

            return result;
        }
    }
}
