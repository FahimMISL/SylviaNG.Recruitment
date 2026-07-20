using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Queries.ExamHallGetActiveLookup
{
    public class ExamHallGetActiveLookupHandler : IRequestHandler<ExamHallGetActiveLookupQuery, List<ExamHallLookupResponse>>
    {
        private readonly IExamHallService _examHallService;

        public ExamHallGetActiveLookupHandler(IExamHallService examHallService)
        {
            _examHallService = examHallService;
        }

        public async Task<List<ExamHallLookupResponse>> Handle(ExamHallGetActiveLookupQuery query, CancellationToken cancellationToken)
        {
            return await _examHallService.GetActiveLookupAsync();
        }
    }
}
