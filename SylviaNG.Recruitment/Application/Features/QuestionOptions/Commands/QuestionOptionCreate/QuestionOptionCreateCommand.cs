using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionOptions.Models;

namespace SylviaNG.Recruitment.Application.Features.QuestionOptions.Commands.QuestionOptionCreate
{
    public class QuestionOptionCreateCommand : IRequest<long>
    {
        public QuestionOptionCreateRequest Request { get; set; }

        public QuestionOptionCreateCommand(QuestionOptionCreateRequest request)
        {
            Request = request;
        }
    }
}
