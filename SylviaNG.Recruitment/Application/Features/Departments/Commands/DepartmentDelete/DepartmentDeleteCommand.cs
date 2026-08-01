using MediatR;

namespace SylviaNG.Recruitment.Application.Features.Departments.Commands.DepartmentDelete
{
    public class DepartmentDeleteCommand : IRequest<Unit>
    {
        public long DepartmentId { get; set; }

        public DepartmentDeleteCommand(long departmentId)
        {
            DepartmentId = departmentId;
        }
    }
}
