using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamAdmitCardDistributeBulk
{
    public class ExamAdmitCardDistributeBulkHandler : IRequestHandler<ExamAdmitCardDistributeBulkCommand, ExamAdmitCardDistributeBulkResponse>
    {
        private readonly IExamNotificationService _examNotificationService;

        public ExamAdmitCardDistributeBulkHandler(IExamNotificationService examNotificationService)
        {
            _examNotificationService = examNotificationService;
        }

        public async Task<ExamAdmitCardDistributeBulkResponse> Handle(ExamAdmitCardDistributeBulkCommand command, CancellationToken cancellationToken)
        {
            var (emailSentCount, smsSentCount, totalCount) = await _examNotificationService.DistributeBulkAsync(command.ExamId);

            return new ExamAdmitCardDistributeBulkResponse
            {
                TotalCount = totalCount,
                EmailSentCount = emailSentCount,
                SmsSentCount = smsSentCount
            };
        }
    }
}
