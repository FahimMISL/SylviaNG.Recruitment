using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamTaking.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamTaking.Commands.ExamTakingStart
{
    public class ExamTakingStartCommand : IRequest<ExamPaperResponse>
    {
        public long ExamEnrollmentId { get; set; }

        public ExamTakingStartCommand(long examEnrollmentId)
        {
            ExamEnrollmentId = examEnrollmentId;
        }
    }
}
