using MediatR;
using SylviaNG.Recruitment.Application.Features.Companies.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Companies.Queries.CompanyGetById
{
    public class CompanyGetByIdHandler : IRequestHandler<CompanyGetByIdQuery, CompanyResponse>
    {
        private readonly ICompanyService _companyService;

        public CompanyGetByIdHandler(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        public async Task<CompanyResponse> Handle(CompanyGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _companyService.GetByIdAsync(query.CompanyId);
        }
    }
}
