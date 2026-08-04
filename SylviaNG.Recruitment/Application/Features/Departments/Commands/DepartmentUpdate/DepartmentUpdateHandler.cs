using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Departments.Commands.DepartmentUpdate
{
    public class DepartmentUpdateHandler : IRequestHandler<DepartmentUpdateCommand, Unit>
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentUpdateHandler(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task<Unit> Handle(DepartmentUpdateCommand command, CancellationToken cancellationToken)
        {
            await _departmentService.UpdateAsync(command.DepartmentId, command.Request);
            return Unit.Value;
        }
    }
}
