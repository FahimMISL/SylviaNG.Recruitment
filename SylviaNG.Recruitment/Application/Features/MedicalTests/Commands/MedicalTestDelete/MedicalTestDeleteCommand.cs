using MediatR;

namespace SylviaNG.Recruitment.Application.Features.MedicalTests.Commands.MedicalTestDelete
{
    public class MedicalTestDeleteCommand : IRequest<Unit>
    {
        public long MedicalTestId { get; set; }

        public MedicalTestDeleteCommand(long medicalTestId)
        {
            MedicalTestId = medicalTestId;
        }
    }
}
