using MediatR;
using SylviaNG.Recruitment.Application.Features.Departments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Departments.Queries.DepartmentGetAll
{
    public class DepartmentGetAllHandler : IRequestHandler<DepartmentGetAllQuery, List<DepartmentResponse>>
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentGetAllHandler(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task<List<DepartmentResponse>> Handle(DepartmentGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _departmentService.GetAllAsync();
        }
    }
}
