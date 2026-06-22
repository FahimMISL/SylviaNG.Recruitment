using MediatR;
using SylviaNG.Recruitment.Application.Features.Questions.Models;

namespace SylviaNG.Recruitment.Application.Features.Questions.Commands.QuestionCreate
{
    public class QuestionCreateCommand : IRequest<long>
    {
        public QuestionCreateRequest Request { get; set; }

        public QuestionCreateCommand(QuestionCreateRequest request)
        {
            Request = request;
        }
    }
}
