using MediatR;
using SylviaNG.Recruitment.Application.Features.MedicalTests.Models;

namespace SylviaNG.Recruitment.Application.Features.MedicalTests.Commands.MedicalTestCreate
{
    public class MedicalTestCreateCommand : IRequest<long>
    {
        public MedicalTestCreateRequest Request { get; set; }

        public MedicalTestCreateCommand(MedicalTestCreateRequest request)
        {
            Request = request;
        }
    }
}
