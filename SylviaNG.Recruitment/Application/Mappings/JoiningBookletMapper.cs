using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class JoiningBookletMapper
    {
        public static JoiningBookletResponse ToResponse(this JoiningBooklet entity)
        {
            return new JoiningBookletResponse
            {
                JoiningBookletId = entity.JoiningBookletId,
                JobApplicationId = entity.JobApplicationId,
                CandidateName = entity.JobApplication?.CandidateName ?? string.Empty,
                OfferLetterId = entity.OfferLetterId,
                DocumentTemplateId = entity.DocumentTemplateId,
                DocumentTemplateName = entity.DocumentTemplate?.Name ?? string.Empty,
                BatchLabel = entity.BatchLabel,
                JoiningDate = entity.JoiningDate,
                RenderedBody = entity.RenderedBody,
                GeneratedPdfPath = entity.GeneratedPdfPath,
                GeneratedAt = entity.GeneratedAt,
            };
        }

        public static JoiningBookletEligibleCandidateResponse ToEligibleCandidateResponse(this OfferLetter entity)
        {
            return new JoiningBookletEligibleCandidateResponse
            {
                OfferLetterId = entity.OfferLetterId,
                JobApplicationId = entity.JobApplicationId,
                CandidateName = entity.JobApplication?.CandidateName ?? string.Empty,
                Designation = entity.Designation,
                OfferJoiningDate = entity.JoiningDate,
            };
        }
    }
}
