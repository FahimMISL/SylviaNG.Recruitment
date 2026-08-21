using System.Text.RegularExpressions;

namespace SylviaNG.Recruitment.Application.Features.CvBank
{
    internal static class CvFileNaming
    {
        public static string ToPdfFileName(string candidateFullName, long candidateProfileId)
        {
            var safeName = Regex.Replace(candidateFullName, @"[^a-zA-Z0-9\-]+", "_").Trim('_');
            if (string.IsNullOrEmpty(safeName))
                safeName = "candidate";

            return $"{safeName}_{candidateProfileId}_CV.pdf";
        }

        /// <summary>US-101 AC3: `CandidateName_ApplicationID.pdf` - different naming convention
        /// from ToPdfFileName above (application-scoped, not profile-scoped), since a bulk CV
        /// download from the application list is keyed by JobApplicationId, not CandidateProfileId.</summary>
        internal static string ToApplicationCvFileName(string candidateName, long jobApplicationId)
        {
            var safeName = Regex.Replace(candidateName, @"[^a-zA-Z0-9\-]+", "_").Trim('_');
            if (string.IsNullOrEmpty(safeName))
                safeName = "candidate";

            return $"{safeName}_{jobApplicationId}.pdf";
        }
    }
}
