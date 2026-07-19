using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Queries.ExamHallGetActiveLookup
{
    public class ExamHallGetActiveLookupQuery : IRequest<List<ExamHallLookupResponse>>
    {
    }
}
