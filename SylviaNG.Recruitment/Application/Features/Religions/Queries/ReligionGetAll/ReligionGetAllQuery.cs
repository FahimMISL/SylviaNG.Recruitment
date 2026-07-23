using MediatR;
using SylviaNG.Recruitment.Application.Features.Religions.Models;

namespace SylviaNG.Recruitment.Application.Features.Religions.Queries.ReligionGetAll
{
    public class ReligionGetAllQuery : IRequest<List<ReligionResponse>>
    {
    }
}
