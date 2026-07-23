using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamVenues.Queries.ExamVenueGetActiveLookup
{
    public class ExamVenueGetActiveLookupQuery : IRequest<List<ExamVenueLookupResponse>>
    {
    }
}
