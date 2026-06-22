using MediatR;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletGetAll
{
    public class JoiningBookletGetAllQuery : IRequest<List<JoiningBookletResponse>>
    {
    }
}
