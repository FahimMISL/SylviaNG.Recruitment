using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamVenues.Queries.ExamVenueGetAll
{
    public class ExamVenueGetAllHandler : IRequestHandler<ExamVenueGetAllQuery, List<ExamVenueResponse>>
    {
        private readonly IExamVenueService _examVenueService;

        public ExamVenueGetAllHandler(IExamVenueService examVenueService)
        {
            _examVenueService = examVenueService;
        }

        public async Task<List<ExamVenueResponse>> Handle(ExamVenueGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _examVenueService.GetAllAsync();
        }
    }
}
