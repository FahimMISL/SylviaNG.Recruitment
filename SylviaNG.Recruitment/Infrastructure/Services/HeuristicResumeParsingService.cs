using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Services
{
    /// <summary>
    /// Free, local, regex/heuristic resume parser (no external AI API). Accuracy is best-effort:
    /// email/phone/full-name are usually reliable, education/experience/skills are rough guesses
    /// the candidate is expected to correct before saving. Also used as the in-process fallback
    /// for AiResumeParsingService when Groq is unavailable (see that class).
    /// </summary>
    public class HeuristicResumeParsingService : IResumeParsingService
    {
        private static readonly Regex EmailRegex = new(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new(@"(\+?\d{1,3}[\s.-]?)?\(?\d{2,4}\)?[\s.-]?\d{3,4}[\s.-]?\d{3,4}", RegexOptions.Compiled);
        private static readonly Regex YearRegex = new(@"(19|20)\d{2}", RegexOptions.Compiled);

        // BD-style CVs/bio-data commonly list these as explicit "Label: value" lines - only
        // extracted when literally present under one of these labels, never guessed from context.
        private static readonly Regex DateOfBirthRegex = new(@"(?:date\s*of\s*birth|d\.?o\.?b\.?)\s*[:\-]\s*(.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex GenderRegex = new(@"gender\s*[:\-]\s*(.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex ReligionRegex = new(@"religion\s*[:\-]\s*(.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex MaritalStatusRegex = new(@"marital\s*status\s*[:\-]\s*(.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly string[] EducationHeaders = { "education", "academic background", "academic qualification" };
        private static readonly string[] ExperienceHeaders = { "experience", "work experience", "employment history", "professional experience" };
        private static readonly string[] SkillsHeaders = { "skills", "technical skills", "core competencies" };
        private static readonly string[] AllSectionHeaders = EducationHeaders.Concat(ExperienceHeaders).Concat(SkillsHeaders)
            .Concat(new[] { "summary", "objective", "certifications", "projects", "references", "languages" }).ToArray();

        public string ProviderName => "Heuristic";

        // Words that mark the institution-name segment of an education line, so it can be split
        // apart from the degree/title segment (see ExtractEducations).
        private static readonly string[] InstitutionKeywords = { "university", "college", "institute", "polytechnic", "school" };

        private readonly IUniversityLibraryItemRepository _universityLibraryItemRepository;

        public HeuristicResumeParsingService(IUniversityLibraryItemRepository universityLibraryItemRepository)
        {
            _universityLibraryItemRepository = universityLibraryItemRepository;
        }

        public async Task<CandidateResumeParseResponse> ParseAsync(IFormFile file)
        {
            var text = await ExtractRawTextAsync(file);

            var lines = text.Split('\n')
                .Select(l => l.Trim())
                .Where(l => l.Length > 0)
                .ToList();

            var sections = SplitIntoSections(lines);
            var universities = await _universityLibraryItemRepository.GetAllOrderedAsync();

            return new CandidateResumeParseResponse
            {
                FullName = ExtractFullName(lines),
                Email = ExtractFirst(EmailRegex, text),
                Phone = ExtractFirst(PhoneRegex, text),
                DateOfBirth = ExtractDateOfBirth(lines),
                Gender = ExtractGender(lines),
                Religion = ExtractReligion(lines),
                MaritalStatus = ExtractMaritalStatus(lines),
                Skills = ExtractSkills(sections),
                Educations = ExtractEducations(sections, universities),
                WorkExperiences = ExtractWorkExperiences(sections),
                ParsingProvider = ProviderName,
                AiParsingDegraded = false
            };
        }

        // Raw text only, no field parsing - shared with JobApplicationService so a submitted
        // resume's text can be persisted for CV Bank search (US-045) without re-implementing
        // PDF/DOCX extraction there.
        public Task<string> ExtractRawTextAsync(IFormFile file) => ResumeTextExtractor.ExtractAsync(file);

        private static DateTime? ExtractDateOfBirth(List<string> lines)
        {
            var value = MatchLabelValue(lines, DateOfBirthRegex);
            if (value == null) return null;

            return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
                ? parsed
                : null;
        }

        // Returns the guessed value as plain text (matching the seeded Gender/Religion/
        // MaritalStatus lookup Name values) rather than an enum - those are now dynamic
        // admin-managed tables, so the parser can't resolve a specific row Id itself. The
        // frontend/candidate matches this text against the loaded dropdown as a suggestion.
        private static string? ExtractGender(List<string> lines)
        {
            var value = MatchLabelValue(lines, GenderRegex);
            if (value == null) return null;

            if (value.StartsWith("male", StringComparison.OrdinalIgnoreCase)) return "Male";
            if (value.StartsWith("female", StringComparison.OrdinalIgnoreCase)) return "Female";
            return "Other";
        }

        private static string? ExtractReligion(List<string> lines)
        {
            var value = MatchLabelValue(lines, ReligionRegex);
            if (value == null) return null;

            if (value.Contains("islam", StringComparison.OrdinalIgnoreCase) || value.Contains("muslim", StringComparison.OrdinalIgnoreCase)) return "Islam";
            if (value.Contains("hindu", StringComparison.OrdinalIgnoreCase)) return "Hinduism";
            if (value.Contains("christian", StringComparison.OrdinalIgnoreCase)) return "Christianity";
            if (value.Contains("buddh", StringComparison.OrdinalIgnoreCase)) return "Buddhism";
            return "Other";
        }

        private static string? ExtractMaritalStatus(List<string> lines)
        {
            var value = MatchLabelValue(lines, MaritalStatusRegex);
            if (value == null) return null;

            if (value.Contains("single", StringComparison.OrdinalIgnoreCase) || value.Contains("unmarried", StringComparison.OrdinalIgnoreCase)) return "Single";
            if (value.Contains("married", StringComparison.OrdinalIgnoreCase) && !value.Contains("unmarried", StringComparison.OrdinalIgnoreCase)) return "Married";
            return "Other";
        }

        // Scans line-by-line (not the whole text) so the captured value stops at the end of the
        // line the label appears on, rather than swallowing unrelated following lines.
        private static string? MatchLabelValue(List<string> lines, Regex labelRegex)
        {
            foreach (var line in lines)
            {
                var match = labelRegex.Match(line);
                if (match.Success)
                {
                    var value = match.Groups[1].Value.Trim();
                    if (value.Length > 0) return value;
                }
            }
            return null;
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

        // Best-effort: one entry per line that looks like "<something> <4-digit year>". If a
        // comma/dash-separated segment contains a university/college/institute keyword, that
        // segment is split out as Institution (and matched against the seeded university list);
        // otherwise the whole line stays in DegreeTitle for the candidate to split/correct.
        private static List<CandidateResumeParsedEducation> ExtractEducations(Dictionary<string, List<string>> sections, List<UniversityLibraryItem> universities)
        {
            var lines = GetSectionLines(sections, EducationHeaders);
            var results = new List<CandidateResumeParsedEducation>();

            foreach (var line in lines)
            {
                if (line.Length < 4) continue;

                var yearMatch = YearRegex.Match(line);
                var (degreeTitle, institution) = SplitDegreeAndInstitution(line);
                var matchedUniversity = institution != null ? MatchUniversity(institution, universities) : null;

                results.Add(new CandidateResumeParsedEducation
                {
                    DegreeTitle = degreeTitle,
                    Institution = institution,
                    UniversityLibraryItemId = matchedUniversity?.UniversityLibraryItemId,
                    PassingYear = yearMatch.Success ? int.Parse(yearMatch.Value) : null
                });
            }

            return results;
        }

        private static (string DegreeTitle, string? Institution) SplitDegreeAndInstitution(string line)
        {
            var segments = line.Split(new[] { ',', '-', '|' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
            var institutionSegment = segments.FirstOrDefault(s => InstitutionKeywords.Any(k => s.Contains(k, StringComparison.OrdinalIgnoreCase)));

            if (institutionSegment == null) return (line, null);

            var remainder = string.Join(", ", segments.Where(s => s != institutionSegment));
            return (remainder.Length > 0 ? remainder : line, institutionSegment);
        }

        // Internal (not private) so AiResumeParsingService can reuse the same matching logic
        // against the institution string its own richer parse already extracted.
        internal static UniversityLibraryItem? MatchUniversity(string institution, List<UniversityLibraryItem> universities)
        {
            return universities.FirstOrDefault(u =>
                institution.Contains(u.Name, StringComparison.OrdinalIgnoreCase) ||
                u.Name.Contains(institution, StringComparison.OrdinalIgnoreCase));
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
