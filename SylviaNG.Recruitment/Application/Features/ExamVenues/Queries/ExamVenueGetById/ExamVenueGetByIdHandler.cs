using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamVenues.Queries.ExamVenueGetById
{
    public class ExamVenueGetByIdHandler : IRequestHandler<ExamVenueGetByIdQuery, ExamVenueResponse>
    {
        private readonly IExamVenueService _examVenueService;

        public ExamVenueGetByIdHandler(IExamVenueService examVenueService)
        {
            _examVenueService = examVenueService;
        }

        public async Task<ExamVenueResponse> Handle(ExamVenueGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _examVenueService.GetByIdAsync(query.ExamVenueId);
        }
    }
}
