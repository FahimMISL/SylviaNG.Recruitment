using MediatR;
using SylviaNG.Recruitment.Application.Features.Exams.Models;

namespace SylviaNG.Recruitment.Application.Features.Exams.Commands.ExamUpdate
{
    public class ExamUpdateCommand : IRequest<Unit>
    {
        public long ExamId { get; set; }
        public ExamUpdateRequest Request { get; set; }

        public ExamUpdateCommand(long examId, ExamUpdateRequest request)
        {
            ExamId = examId;
            Request = request;
        }
    }
}
