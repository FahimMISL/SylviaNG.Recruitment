using MediatR;
using SylviaNG.Recruitment.Application.Features.MedicalLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.MedicalLetters.Queries.MedicalLetterGetAll
{
    public class MedicalLetterGetAllQuery : IRequest<List<MedicalLetterResponse>>
    {
        public long? JobApplicationId { get; set; }

        public MedicalLetterGetAllQuery(long? jobApplicationId)
        {
            JobApplicationId = jobApplicationId;
        }
    }
}
