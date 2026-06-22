using MediatR;
using SylviaNG.Recruitment.Application.Features.Nominees.Models;

namespace SylviaNG.Recruitment.Application.Features.Nominees.Queries.NomineeGetAll
{
    public class NomineeGetAllQuery : IRequest<List<NomineeResponse>>
    {
    }
}
