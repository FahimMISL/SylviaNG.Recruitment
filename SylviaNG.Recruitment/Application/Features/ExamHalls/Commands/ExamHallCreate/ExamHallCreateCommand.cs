using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallCreate
{
    public class ExamHallCreateCommand : IRequest<long>
    {
        public ExamHallCreateRequest Request { get; set; }

        public ExamHallCreateCommand(ExamHallCreateRequest request)
        {
            Request = request;
        }
    }
}
