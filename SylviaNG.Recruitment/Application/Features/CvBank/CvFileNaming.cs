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
    }
}
