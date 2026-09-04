using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamSeatPlanGenerate
{
    public class ExamSeatPlanGenerateCommand : IRequest
    {
        public long ExamId { get; set; }

        public ExamSeatPlanGenerateCommand(long examId)
        {
            ExamId = examId;
        }
    }
}
