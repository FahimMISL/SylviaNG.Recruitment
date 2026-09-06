using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Departments.Commands.DepartmentDelete
{
    public class DepartmentDeleteHandler : IRequestHandler<DepartmentDeleteCommand, Unit>
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentDeleteHandler(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task<Unit> Handle(DepartmentDeleteCommand command, CancellationToken cancellationToken)
        {
            await _departmentService.DeleteAsync(command.DepartmentId);
            return Unit.Value;
        }
    }
}
