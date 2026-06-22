using MediatR;
using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Models;

namespace SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Queries.ImpersonationLogGetAll
{
    public class ImpersonationLogGetAllQuery : IRequest<List<ImpersonationLogResponse>>
    {
    }
}
