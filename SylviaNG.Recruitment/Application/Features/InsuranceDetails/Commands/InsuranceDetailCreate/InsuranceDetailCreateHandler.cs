using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InsuranceDetails.Commands.InsuranceDetailCreate
{
    public class InsuranceDetailCreateHandler : IRequestHandler<InsuranceDetailCreateCommand, long>
    {
        private readonly IInsuranceDetailService _service;

        public InsuranceDetailCreateHandler(IInsuranceDetailService service)
        {
            _service = service;
        }

        public async Task<long> Handle(InsuranceDetailCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
