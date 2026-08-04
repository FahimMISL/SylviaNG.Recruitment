using MediatR;
using SylviaNG.Recruitment.Application.Features.Departments.Models;

namespace SylviaNG.Recruitment.Application.Features.Departments.Commands.DepartmentUpdate
{
    public class DepartmentUpdateCommand : IRequest<Unit>
    {
        public long DepartmentId { get; set; }
        public DepartmentUpdateRequest Request { get; set; }

        public DepartmentUpdateCommand(long departmentId, DepartmentUpdateRequest request)
        {
            DepartmentId = departmentId;
            Request = request;
        }
    }
}
