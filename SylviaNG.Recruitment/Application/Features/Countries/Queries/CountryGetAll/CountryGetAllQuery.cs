using MediatR;
using SylviaNG.Recruitment.Application.Features.Countries.Models;

namespace SylviaNG.Recruitment.Application.Features.Countries.Queries.CountryGetAll
{
    public class CountryGetAllQuery : IRequest<List<CountryResponse>>
    {
    }
}
