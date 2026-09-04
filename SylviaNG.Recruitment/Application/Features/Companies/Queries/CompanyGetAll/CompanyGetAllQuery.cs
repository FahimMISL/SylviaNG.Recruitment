using MediatR;
using SylviaNG.Recruitment.Application.Features.Companies.Models;

namespace SylviaNG.Recruitment.Application.Features.Companies.Queries.CompanyGetAll
{
    public class CompanyGetAllQuery : IRequest<List<CompanyResponse>>
    {
    }
}
