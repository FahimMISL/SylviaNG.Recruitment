using MediatR;
using SylviaNG.Recruitment.Application.Features.MedicalTests.Models;

namespace SylviaNG.Recruitment.Application.Features.MedicalTests.Commands.MedicalTestUpdate
{
    public class MedicalTestUpdateCommand : IRequest<Unit>
    {
        public long MedicalTestId { get; set; }
        public MedicalTestUpdateRequest Request { get; set; }

        public MedicalTestUpdateCommand(long medicalTestId, MedicalTestUpdateRequest request)
        {
            MedicalTestId = medicalTestId;
            Request = request;
        }
    }
}
