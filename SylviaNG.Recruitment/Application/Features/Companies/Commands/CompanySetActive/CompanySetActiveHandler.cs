using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Companies.Commands.CompanySetActive
{
    public class CompanySetActiveHandler : IRequestHandler<CompanySetActiveCommand, Unit>
    {
        private readonly ICompanyService _companyService;

        public CompanySetActiveHandler(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        public async Task<Unit> Handle(CompanySetActiveCommand command, CancellationToken cancellationToken)
        {
            await _companyService.SetActiveAsync(command.CompanyId, command.IsActive);
            return Unit.Value;
        }
    }
}
