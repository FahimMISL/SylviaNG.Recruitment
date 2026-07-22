using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupCreate
{
    public class QuestionGroupCreateCommand : IRequest<long>
    {
        public QuestionGroupCreateRequest Request { get; set; }

        public QuestionGroupCreateCommand(QuestionGroupCreateRequest request)
        {
            Request = request;
        }
    }
}
