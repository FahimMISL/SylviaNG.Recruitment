using MediatR;
using SylviaNG.Recruitment.Application.Features.Genders.Models;

namespace SylviaNG.Recruitment.Application.Features.Genders.Queries.GenderGetAll
{
    public class GenderGetAllQuery : IRequest<List<GenderResponse>>
    {
    }
}
