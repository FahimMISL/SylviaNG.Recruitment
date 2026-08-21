using MediatR;
using SylviaNG.Recruitment.Application.Features.Departments.Models;

namespace SylviaNG.Recruitment.Application.Features.Departments.Commands.DepartmentCreate
{
    public class DepartmentCreateCommand : IRequest<long>
    {
        public DepartmentCreateRequest Request { get; set; }

        public DepartmentCreateCommand(DepartmentCreateRequest request)
        {
            Request = request;
        }
    }
}
