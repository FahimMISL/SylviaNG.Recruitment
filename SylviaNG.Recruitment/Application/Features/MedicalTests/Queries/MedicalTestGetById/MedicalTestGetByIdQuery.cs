using MediatR;
using SylviaNG.Recruitment.Application.Features.MedicalTests.Models;

namespace SylviaNG.Recruitment.Application.Features.MedicalTests.Queries.MedicalTestGetById
{
    public class MedicalTestGetByIdQuery : IRequest<MedicalTestResponse>
    {
        public long MedicalTestId { get; set; }

        public MedicalTestGetByIdQuery(long medicalTestId)
        {
            MedicalTestId = medicalTestId;
        }
    }
}
