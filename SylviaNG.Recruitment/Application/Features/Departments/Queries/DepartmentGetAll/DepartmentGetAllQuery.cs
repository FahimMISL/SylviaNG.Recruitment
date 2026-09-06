using MediatR;
using SylviaNG.Recruitment.Application.Features.Departments.Models;

namespace SylviaNG.Recruitment.Application.Features.Departments.Queries.DepartmentGetAll
{
    public class DepartmentGetAllQuery : IRequest<List<DepartmentResponse>>
    {
    }
}
