using SylviaNG.Recruitment.Application.Common.Helpers;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class OfferLetterMapper
    {
        public static OfferLetterResponse ToResponse(this OfferLetter entity)
        {
            return new OfferLetterResponse
            {
                OfferLetterId = entity.OfferLetterId,
                JobApplicationId = entity.JobApplicationId,
                CandidateName = entity.JobApplication?.CandidateName ?? string.Empty,
                DocumentTemplateId = entity.DocumentTemplateId,
                DocumentTemplateName = entity.DocumentTemplate?.Name ?? string.Empty,
                Designation = entity.Designation,
                OfferedSalary = entity.OfferedSalary,
                JoiningDate = entity.JoiningDate,
                ReportingManager = entity.ReportingManager,
                OfferValidityDate = entity.OfferValidityDate,
                GeneratedPdfPath = FileUrlBuilder.BuildDownloadUrl(entity.GeneratedPdfPath) ?? string.Empty,
                Status = entity.Status,
                GeneratedAt = entity.GeneratedAt,
                DecisionAt = entity.DecisionAt,
                DeclineReason = entity.DeclineReason,
            };
        }
    }
}
