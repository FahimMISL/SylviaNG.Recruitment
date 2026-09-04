using MediatR;
using SylviaNG.Recruitment.Application.Features.AddressLookup.Models;

namespace SylviaNG.Recruitment.Application.Features.AddressLookup.Queries.DivisionGetAll
{
    public class DivisionGetAllQuery : IRequest<List<DivisionResponse>>
    {
    }
}
