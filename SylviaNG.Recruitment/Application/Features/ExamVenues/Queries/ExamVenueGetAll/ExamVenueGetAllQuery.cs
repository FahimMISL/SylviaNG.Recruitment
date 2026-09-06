using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamVenues.Queries.ExamVenueGetAll
{
    public class ExamVenueGetAllQuery : IRequest<List<ExamVenueResponse>>
    {
    }
}
