using MediatR;
using SylviaNG.Recruitment.Application.Features.Companies.Models;

namespace SylviaNG.Recruitment.Application.Features.Companies.Queries.CompanyGetById
{
    public class CompanyGetByIdQuery : IRequest<CompanyResponse>
    {
        public long CompanyId { get; set; }

        public CompanyGetByIdQuery(long companyId)
        {
            CompanyId = companyId;
        }
    }
}
