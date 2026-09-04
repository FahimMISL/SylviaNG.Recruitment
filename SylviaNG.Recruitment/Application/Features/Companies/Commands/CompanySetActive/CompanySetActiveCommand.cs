using MediatR;

namespace SylviaNG.Recruitment.Application.Features.Companies.Commands.CompanySetActive
{
    public class CompanySetActiveCommand : IRequest<Unit>
    {
        public long CompanyId { get; set; }
        public bool IsActive { get; set; }

        public CompanySetActiveCommand(long companyId, bool isActive)
        {
            CompanyId = companyId;
            IsActive = isActive;
        }
    }
}
