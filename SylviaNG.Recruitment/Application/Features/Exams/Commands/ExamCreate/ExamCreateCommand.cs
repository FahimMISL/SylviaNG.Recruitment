using MediatR;
using SylviaNG.Recruitment.Application.Features.Exams.Models;

namespace SylviaNG.Recruitment.Application.Features.Exams.Commands.ExamCreate
{
    public class ExamCreateCommand : IRequest<long>
    {
        public ExamCreateRequest Request { get; set; }

        public ExamCreateCommand(ExamCreateRequest request)
        {
            Request = request;
        }
    }
}
