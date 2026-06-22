using MediatR;
using SylviaNG.Recruitment.Application.Features.MedicalTests.Models;

namespace SylviaNG.Recruitment.Application.Features.MedicalTests.Queries.MedicalTestGetAll
{
    public class MedicalTestGetAllQuery : IRequest<List<MedicalTestResponse>>
    {
    }
}
