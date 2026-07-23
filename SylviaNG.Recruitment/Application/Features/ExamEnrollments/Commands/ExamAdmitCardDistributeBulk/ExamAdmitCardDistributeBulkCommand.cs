using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamAdmitCardDistributeBulk
{
    public class ExamAdmitCardDistributeBulkCommand : IRequest<ExamAdmitCardDistributeBulkResponse>
    {
        public long ExamId { get; set; }

        public ExamAdmitCardDistributeBulkCommand(long examId)
        {
            ExamId = examId;
        }
    }
}
