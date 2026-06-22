using MediatR;
using SylviaNG.Recruitment.Application.Features.Requisitions.Models;

namespace SylviaNG.Recruitment.Application.Features.Requisitions.Queries.RequisitionGetAll
{
    public class RequisitionGetAllQuery : IRequest<List<RequisitionResponse>>
    {
    }
}
