using System.Net;
using System.Text.RegularExpressions;

namespace SylviaNG.Recruitment.Application.Common.Email
{
    /// <summary>
    /// Every NotificationTemplate.Body is authored as plain text (blank line = new paragraph),
    /// but EmailMessage.HtmlBody is sent as-is to an HTML mail client - without this conversion
    /// every line break collapses and the whole template renders as one run-on paragraph.
    /// Templates that already contain real markup are left untouched. Shared by every
    /// ISmtpEmailService implementation (SmtpEmailService, BrevoEmailService, ...).
    /// </summary>
    public static class PlainTextEmailFormatter
    {
        private static readonly Regex HtmlTagRegex = new(@"<[a-zA-Z][^>]*>", RegexOptions.Compiled);
        private static readonly Regex ParagraphBreakRegex = new(@"(\r?\n){2,}", RegexOptions.Compiled);

        public static string ToHtml(string body)
        {
            if (string.IsNullOrEmpty(body) || HtmlTagRegex.IsMatch(body))
                return body;

            var encoded = WebUtility.HtmlEncode(body);
            var paragraphs = ParagraphBreakRegex.Split(encoded)
                .Select(p => p.Trim())
                .Where(p => p.Length > 0)
                .Select(p => $"<p>{p.Replace("\n", "<br>")}</p>");

            return string.Concat(paragraphs);
        }
    }
}
