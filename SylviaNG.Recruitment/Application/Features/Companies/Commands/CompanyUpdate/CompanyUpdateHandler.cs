using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Companies.Commands.CompanyUpdate
{
    public class CompanyUpdateHandler : IRequestHandler<CompanyUpdateCommand, Unit>
    {
        private readonly ICompanyService _companyService;

        public CompanyUpdateHandler(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        public async Task<Unit> Handle(CompanyUpdateCommand command, CancellationToken cancellationToken)
        {
            await _companyService.UpdateAsync(command.CompanyId, command.Request);
            return Unit.Value;
        }
    }
}
