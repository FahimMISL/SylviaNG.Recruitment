using MediatR;
using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Models;

namespace SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Queries.ApplicationDuplicateGetAll
{
    public class ApplicationDuplicateGetAllQuery : IRequest<List<ApplicationDuplicateResponse>>
    {
    }
}
