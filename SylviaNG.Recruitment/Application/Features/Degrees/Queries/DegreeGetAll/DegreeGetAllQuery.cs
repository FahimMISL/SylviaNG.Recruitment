using MediatR;
using SylviaNG.Recruitment.Application.Features.Degrees.Models;

namespace SylviaNG.Recruitment.Application.Features.Degrees.Queries.DegreeGetAll
{
    public class DegreeGetAllQuery : IRequest<List<DegreeResponse>>
    {
    }
}
