using MediatR;
using SylviaNG.Recruitment.Application.Features.Companies.Models;

namespace SylviaNG.Recruitment.Application.Features.Companies.Commands.CompanyUpdate
{
    public class CompanyUpdateCommand : IRequest<Unit>
    {
        public long CompanyId { get; set; }
        public CompanyUpdateRequest Request { get; set; }

        public CompanyUpdateCommand(long companyId, CompanyUpdateRequest request)
        {
            CompanyId = companyId;
            Request = request;
        }
    }
}
