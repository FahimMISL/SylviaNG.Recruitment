using MediatR;
using SylviaNG.Recruitment.Application.Features.MedicalLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.MedicalLetters.Queries.MedicalLetterGetById
{
    public class MedicalLetterGetByIdQuery : IRequest<MedicalLetterResponse>
    {
        public long MedicalLetterId { get; set; }

        public MedicalLetterGetByIdQuery(long medicalLetterId)
        {
            MedicalLetterId = medicalLetterId;
        }
    }
}
