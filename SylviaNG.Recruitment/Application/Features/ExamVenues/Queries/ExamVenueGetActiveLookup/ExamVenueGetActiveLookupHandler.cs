using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamVenues.Queries.ExamVenueGetActiveLookup
{
    public class ExamVenueGetActiveLookupHandler : IRequestHandler<ExamVenueGetActiveLookupQuery, List<ExamVenueLookupResponse>>
    {
        private readonly IExamVenueService _examVenueService;

        public ExamVenueGetActiveLookupHandler(IExamVenueService examVenueService)
        {
            _examVenueService = examVenueService;
        }

        public async Task<List<ExamVenueLookupResponse>> Handle(ExamVenueGetActiveLookupQuery query, CancellationToken cancellationToken)
        {
            return await _examVenueService.GetActiveLookupAsync();
        }
    }
}
