using System.Text.RegularExpressions;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Common.Notifications
{
    /// <summary>
    /// EP-09 US-073: {{Token}} placeholder engine shared by every NotificationTemplate render -
    /// admin preview (Feature 1) and real send (Feature 2) both go through this. No prior
    /// template-substitution code existed anywhere in the codebase; ExamNotificationService and
    /// InterviewNotificationService hardcode their bodies via plain C# string interpolation.
    /// </summary>
    public class PlaceholderSubstitutionService : IPlaceholderSubstitutionService
    {
        private static readonly Regex PlaceholderPattern = new(@"\{\{\s*([A-Za-z0-9_]+)\s*\}\}", RegexOptions.Compiled);

        public string Render(string template, IDictionary<string, string> values)
        {
            if (string.IsNullOrEmpty(template))
                return template ?? string.Empty;

            var lookup = new Dictionary<string, string>(values, StringComparer.OrdinalIgnoreCase);

            return PlaceholderPattern.Replace(template, match =>
            {
                var token = match.Groups[1].Value;
                return lookup.TryGetValue(token, out var value) ? value : match.Value;
            });
        }

        public List<string> ExtractPlaceholders(string template)
        {
            if (string.IsNullOrEmpty(template))
                return new List<string>();

            var seen = new List<string>();
            foreach (Match match in PlaceholderPattern.Matches(template))
            {
                var token = match.Groups[1].Value;
                if (!seen.Contains(token, StringComparer.OrdinalIgnoreCase))
                    seen.Add(token);
            }

            return seen;
        }
    }
}
