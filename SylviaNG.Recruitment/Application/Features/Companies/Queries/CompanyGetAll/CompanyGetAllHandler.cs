using MediatR;
using SylviaNG.Recruitment.Application.Features.Companies.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Companies.Queries.CompanyGetAll
{
    public class CompanyGetAllHandler : IRequestHandler<CompanyGetAllQuery, List<CompanyResponse>>
    {
        private readonly ICompanyService _companyService;

        public CompanyGetAllHandler(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        public async Task<List<CompanyResponse>> Handle(CompanyGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _companyService.GetAllAsync();
        }
    }
}
