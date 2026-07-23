using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Renders a candidate's profile data into a standardized CV/resume PDF - every candidate's
    /// CV follows the same template and section order, since it is generated from structured
    /// profile fields rather than an uploaded resume file.
    /// </summary>
    public interface ICvPdfGeneratorService
    {
        byte[] Generate(CandidateProfile profile);
    }
}
