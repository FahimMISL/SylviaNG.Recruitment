using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Queries.QuestionGroupGetActiveLookup
{
    public class QuestionGroupGetActiveLookupHandler : IRequestHandler<QuestionGroupGetActiveLookupQuery, List<QuestionGroupLookupResponse>>
    {
        private readonly IQuestionGroupService _questionGroupService;

        public QuestionGroupGetActiveLookupHandler(IQuestionGroupService questionGroupService)
        {
            _questionGroupService = questionGroupService;
        }

        public async Task<List<QuestionGroupLookupResponse>> Handle(QuestionGroupGetActiveLookupQuery query, CancellationToken cancellationToken)
        {
            return await _questionGroupService.GetActiveLookupAsync();
        }
    }
}
