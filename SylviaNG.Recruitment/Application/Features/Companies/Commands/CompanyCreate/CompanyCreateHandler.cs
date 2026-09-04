using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Companies.Commands.CompanyCreate
{
    public class CompanyCreateHandler : IRequestHandler<CompanyCreateCommand, long>
    {
        private readonly ICompanyService _companyService;

        public CompanyCreateHandler(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        public async Task<long> Handle(CompanyCreateCommand command, CancellationToken cancellationToken)
        {
            return await _companyService.CreateAsync(command.Request);
        }
    }
}
