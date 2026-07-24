using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamTaking.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamTaking.Commands.ExamTakingSubmit
{
    public class ExamTakingSubmitCommand : IRequest<ExamSubmitResultResponse>
    {
        public long ExamEnrollmentId { get; set; }
        public ExamSubmitRequest Request { get; set; }

        public ExamTakingSubmitCommand(long examEnrollmentId, ExamSubmitRequest request)
        {
            ExamEnrollmentId = examEnrollmentId;
            Request = request;
        }
    }
}
