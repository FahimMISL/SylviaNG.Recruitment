using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallUpdate
{
    public class ExamHallUpdateCommand : IRequest<Unit>
    {
        public long ExamHallId { get; set; }
        public ExamHallUpdateRequest Request { get; set; }

        public ExamHallUpdateCommand(long examHallId, ExamHallUpdateRequest request)
        {
            ExamHallId = examHallId;
            Request = request;
        }
    }
}
