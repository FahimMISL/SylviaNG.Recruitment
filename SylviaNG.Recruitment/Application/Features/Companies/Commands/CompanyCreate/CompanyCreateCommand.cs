using MediatR;
using SylviaNG.Recruitment.Application.Features.Companies.Models;

namespace SylviaNG.Recruitment.Application.Features.Companies.Commands.CompanyCreate
{
    public class CompanyCreateCommand : IRequest<long>
    {
        public CompanyCreateRequest Request { get; set; }

        public CompanyCreateCommand(CompanyCreateRequest request)
        {
            Request = request;
        }
    }
}
