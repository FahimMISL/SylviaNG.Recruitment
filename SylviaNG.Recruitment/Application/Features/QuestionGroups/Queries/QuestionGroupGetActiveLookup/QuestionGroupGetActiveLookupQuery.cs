using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Queries.QuestionGroupGetActiveLookup
{
    public class QuestionGroupGetActiveLookupQuery : IRequest<List<QuestionGroupLookupResponse>>
    {
    }
}
