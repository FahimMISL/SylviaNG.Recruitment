using MediatR;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Models;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Queries.MajorSubjectSscHscGetAll
{
    public class MajorSubjectSscHscGetAllQuery : IRequest<List<MajorSubjectSscHscResponse>>
    {
    }
}
