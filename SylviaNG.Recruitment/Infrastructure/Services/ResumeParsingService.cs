using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using UglyToad.PdfPig;

namespace SylviaNG.Recruitment.Infrastructure.Services
{
    /// <summary>
    /// Free, local, regex/heuristic resume parser (no external AI API). Accuracy is best-effort:
    /// email/phone/full-name are usually reliable, education/experience/skills are rough guesses
    /// the candidate is expected to correct before saving.
    /// </summary>
    public class ResumeParsingService : IResumeParsingService
    {
        private static readonly Regex EmailRegex = new(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new(@"(\+?\d{1,3}[\s.-]?)?\(?\d{2,4}\)?[\s.-]?\d{3,4}[\s.-]?\d{3,4}", RegexOptions.Compiled);
        private static readonly Regex YearRegex = new(@"(19|20)\d{2}", RegexOptions.Compiled);

        private static readonly string[] EducationHeaders = { "education", "academic background", "academic qualification" };
        private static readonly string[] ExperienceHeaders = { "experience", "work experience", "employment history", "professional experience" };
        private static readonly string[] SkillsHeaders = { "skills", "technical skills", "core competencies" };
        private static readonly string[] AllSectionHeaders = EducationHeaders.Concat(ExperienceHeaders).Concat(SkillsHeaders)
            .Concat(new[] { "summary", "objective", "certifications", "projects", "references", "languages" }).ToArray();

        public async Task<CandidateResumeParseResponse> ParseAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            var text = extension switch
            {
                ".pdf" => await ExtractPdfTextAsync(file),
                ".docx" => await ExtractDocxTextAsync(file),
                // The FluentValidation validator already restricts uploads to .pdf/.docx before
                // this runs, so this branch should be unreachable via the API.
                _ => throw new ArgumentException($"Unsupported resume file type: {extension}. Use .pdf or .docx.")
            };

            var lines = text.Split('\n')
                .Select(l => l.Trim())
                .Where(l => l.Length > 0)
                .ToList();

            var sections = SplitIntoSections(lines);

            return new CandidateResumeParseResponse
            {
                FullName = ExtractFullName(lines),
                Email = ExtractFirst(EmailRegex, text),
                Phone = ExtractFirst(PhoneRegex, text),
                Skills = ExtractSkills(sections),
                Educations = ExtractEducations(sections),
                WorkExperiences = ExtractWorkExperiences(sections)
            };
        }

        private static async Task<string> ExtractPdfTextAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            using var document = PdfDocument.Open(stream);
            var sb = new StringBuilder();
            foreach (var page in document.GetPages())
                sb.AppendLine(page.Text);
            return sb.ToString();
        }

        private static async Task<string> ExtractDocxTextAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            using var wordDocument = WordprocessingDocument.Open(stream, false);
            return wordDocument.MainDocumentPart?.Document?.Body?.InnerText ?? string.Empty;
        }

        private static string? ExtractFirst(Regex regex, string text)
        {
            var match = regex.Match(text);
            return match.Success ? match.Value.Trim() : null;
        }

        // Heuristic: the candidate's name is usually the first short line that isn't an email,
        // phone number, or section header.
        private static string? ExtractFullName(List<string> lines)
        {
            foreach (var line in lines.Take(10))
            {
                if (line.Length is < 3 or > 60) continue;
                if (EmailRegex.IsMatch(line) || line.Any(char.IsDigit)) continue;
                if (AllSectionHeaders.Any(h => line.Equals(h, StringComparison.OrdinalIgnoreCase))) continue;

                return line;
            }
            return null;
        }

        // Splits the resume into (headerKey, contentLines) blocks by scanning for lines that
        // are (close to) one of the known section header keywords.
        private static Dictionary<string, List<string>> SplitIntoSections(List<string> lines)
        {
            var sections = new Dictionary<string, List<string>>();
            string? currentHeader = null;

            foreach (var line in lines)
            {
                var matchedHeader = AllSectionHeaders.FirstOrDefault(h =>
                    line.Equals(h, StringComparison.OrdinalIgnoreCase) ||
                    (line.Length < 40 && line.Contains(h, StringComparison.OrdinalIgnoreCase)));

                if (matchedHeader != null)
                {
                    currentHeader = matchedHeader;
                    sections[currentHeader] = new List<string>();
                    continue;
                }

                if (currentHeader != null)
                    sections[currentHeader].Add(line);
            }

            return sections;
        }

        private static List<string> GetSectionLines(Dictionary<string, List<string>> sections, string[] headerAliases)
        {
            var key = sections.Keys.FirstOrDefault(k => headerAliases.Contains(k, StringComparer.OrdinalIgnoreCase));
            return key != null ? sections[key] : new List<string>();
        }

        private static List<string> ExtractSkills(Dictionary<string, List<string>> sections)
        {
            var lines = GetSectionLines(sections, SkillsHeaders);
            return lines
                .SelectMany(l => l.Split(new[] { ',', ';', '|', '•', '·' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(s => s.Trim())
                .Where(s => s.Length is > 1 and < 50)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        // Best-effort: one entry per line that looks like "<something> <4-digit year>", degree
        // and institution left as the same raw line text for the candidate to split/correct.
        private static List<CandidateResumeParsedEducation> ExtractEducations(Dictionary<string, List<string>> sections)
        {
            var lines = GetSectionLines(sections, EducationHeaders);
            var results = new List<CandidateResumeParsedEducation>();

            foreach (var line in lines)
            {
                if (line.Length < 4) continue;

                var yearMatch = YearRegex.Match(line);
                results.Add(new CandidateResumeParsedEducation
                {
                    DegreeTitle = line,
                    Institution = null,
                    PassingYear = yearMatch.Success ? int.Parse(yearMatch.Value) : null
                });
            }

            return results;
        }

        // Best-effort: one entry per non-empty line under the Experience header, raw text kept
        // in Designation so the candidate can split company/role/dates themselves.
        private static List<CandidateResumeParsedWorkExperience> ExtractWorkExperiences(Dictionary<string, List<string>> sections)
        {
            var lines = GetSectionLines(sections, ExperienceHeaders);
            return lines
                .Where(l => l.Length >= 4)
                .Select(l => new CandidateResumeParsedWorkExperience { CompanyName = null, Designation = l })
                .ToList();
        }
    }
}
