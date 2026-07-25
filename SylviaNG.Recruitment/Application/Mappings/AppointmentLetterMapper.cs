using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class AppointmentLetterMapper
    {
        public static AppointmentLetterResponse ToResponse(this AppointmentLetter entity)
        {
            return new AppointmentLetterResponse
            {
                AppointmentLetterId = entity.AppointmentLetterId,
                JobApplicationId = entity.JobApplicationId,
                CandidateName = entity.JobApplication?.CandidateName ?? string.Empty,
                OfferLetterId = entity.OfferLetterId,
                DocumentTemplateId = entity.DocumentTemplateId,
                DocumentTemplateName = entity.DocumentTemplate?.Name ?? string.Empty,
                FinalBody = entity.FinalBody,
                GeneratedPdfPath = entity.GeneratedPdfPath,
                GeneratedAt = entity.GeneratedAt,
            };
        }
    }
}
