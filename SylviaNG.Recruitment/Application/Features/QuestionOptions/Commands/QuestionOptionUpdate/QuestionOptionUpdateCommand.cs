using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionOptions.Models;

namespace SylviaNG.Recruitment.Application.Features.QuestionOptions.Commands.QuestionOptionUpdate
{
    public class QuestionOptionUpdateCommand : IRequest<Unit>
    {
        public long QuestionOptionId { get; set; }
        public QuestionOptionUpdateRequest Request { get; set; }

        public QuestionOptionUpdateCommand(long questionOptionId, QuestionOptionUpdateRequest request)
        {
            QuestionOptionId = questionOptionId;
            Request = request;
        }
    }
}
