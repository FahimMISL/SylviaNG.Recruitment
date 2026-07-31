using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupUpdate
{
    public class QuestionGroupUpdateCommand : IRequest<Unit>
    {
        public long QuestionGroupId { get; set; }
        public QuestionGroupUpdateRequest Request { get; set; }

        public QuestionGroupUpdateCommand(long questionGroupId, QuestionGroupUpdateRequest request)
        {
            QuestionGroupId = questionGroupId;
            Request = request;
        }
    }
}
