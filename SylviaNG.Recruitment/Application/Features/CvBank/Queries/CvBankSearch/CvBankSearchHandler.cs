using System.Text;
using MediatR;
using SylviaNG.Recruitment.Application.Common.BooleanQuery;
using SylviaNG.Recruitment.Application.Features.CvBank.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CvBank.Queries.CvBankSearch
{
    /// <summary>
    /// In-memory, full-scan Boolean search + structured filtering over every active candidate
    /// profile (US-045). Same architectural precedent as ShortlistFilterEvaluationService: the
    /// candidate pool is bounded, so evaluating facts/haystack per-profile in memory is simpler
    /// and more flexible than translating Boolean AND/OR/NOT into SQL.
    /// </summary>
    public class CvBankSearchHandler : IRequestHandler<CvBankSearchQuery, PagedResult<CvBankSearchResultResponse>>
    {
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;

        public CvBankSearchHandler(
            ICandidateProfileRepository candidateProfileRepository,
            IJobApplicationRepository jobApplicationRepository)
        {
            _candidateProfileRepository = candidateProfileRepository;
            _jobApplicationRepository = jobApplicationRepository;
        }

        public async Task<PagedResult<CvBankSearchResultResponse>> Handle(CvBankSearchQuery query, CancellationToken cancellationToken)
        {
            var request = query.Request;

            var booleanQuery = ParseBooleanQuery(request.BooleanQuery);

            var profiles = await _candidateProfileRepository.GetAllActiveWithDetailsAsync();
            var (sourcesByEmail, resumeTextByEmail) = await BuildJobApplicationLookupsAsync();

            var matches = new List<(CandidateProfile Profile, CandidateFactService.CandidateFacts Facts, int RelevanceScore)>();

            foreach (var profile in profiles)
            {
                var facts = CandidateFactService.BuildFacts(profile);

                if (!PassesStructuredFilters(profile, facts, request, sourcesByEmail))
                    continue;

                var resumeText = resumeTextByEmail.TryGetValue(profile.Email, out var text) ? text : string.Empty;
                var haystack = BuildHaystack(profile, resumeText);

                var relevanceScore = 0;
                if (booleanQuery != null)
                {
                    if (!booleanQuery.Evaluate(haystack))
                        continue;

                    relevanceScore = BooleanQueryParser.CountMatchedTerms(booleanQuery, haystack);
                }

                matches.Add((profile, facts, relevanceScore));
            }

            var ordered = matches
                .OrderByDescending(m => m.RelevanceScore)
                .ThenBy(m => m.Profile.FullName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var totalCount = ordered.Count;
            var page = ordered
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(m => ToResponse(m.Profile, m.Facts, m.RelevanceScore))
                .ToList();

            return new PagedResult<CvBankSearchResultResponse>
            {
                Data = page,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        // Malformed queries (bad AND/OR/NOT syntax, unbalanced parens) are a user input error,
        // not a server fault - surfaced as a 400 with a helpful message via the same
        // FluentValidation.ValidationException path GlobalExceptionHandlerMiddleware already
        // maps for every other "bad request" case in this codebase.
        private static IBooleanQueryNode? ParseBooleanQuery(string? booleanQuery)
        {
            if (string.IsNullOrWhiteSpace(booleanQuery))
                return null;

            try
            {
                return BooleanQueryParser.Parse(booleanQuery);
            }
            catch (Exception ex) when (ex is FormatException or ArgumentException)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(nameof(CvBankSearchRequest.BooleanQuery), ex.Message)
                });
            }
        }

        private static bool PassesStructuredFilters(
            CandidateProfile profile,
            CandidateFactService.CandidateFacts facts,
            CvBankSearchRequest request,
            Dictionary<string, HashSet<ApplicationSourceEnum>> sourcesByEmail)
        {
            if (request.EducationLevel.HasValue
                && !facts.EducationLevels.Any(l => (int)l >= (int)request.EducationLevel.Value))
                return false;

            if (request.MinExperienceYears.HasValue && facts.TotalExperienceYears < request.MinExperienceYears.Value)
                return false;

            if (request.MaxExperienceYears.HasValue && facts.TotalExperienceYears > request.MaxExperienceYears.Value)
                return false;

            if (!string.IsNullOrWhiteSpace(request.Location)
                && !facts.AddressText.Contains(request.Location, StringComparison.OrdinalIgnoreCase))
                return false;

            if (request.CandidateType.HasValue)
            {
                if (!sourcesByEmail.TryGetValue(profile.Email, out var sources) || !sources.Contains(request.CandidateType.Value))
                    return false;
            }

            return true;
        }

        private static string BuildHaystack(CandidateProfile profile, string resumeText)
        {
            var parts = new List<string> { profile.FullName, profile.Email, profile.Phone ?? string.Empty };

            parts.AddRange(profile.Skills.Select(s => s.SkillName));
            parts.AddRange(profile.Educations.SelectMany(e => new[] { e.DegreeTitle, e.Institution, e.MajorSubject }).Where(s => s != null)!);
            parts.AddRange(profile.WorkExperiences.SelectMany(w => new[] { w.Designation, w.CompanyName, w.Responsibilities }));
            parts.AddRange(profile.Certifications.SelectMany(c => new[] { c.CertificationName, c.IssuingOrganization }).Where(s => s != null)!);
            parts.Add(resumeText);

            return string.Join(" \n ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        private async Task<(Dictionary<string, HashSet<ApplicationSourceEnum>> SourcesByEmail, Dictionary<string, string> ResumeTextByEmail)>
            BuildJobApplicationLookupsAsync()
        {
            var applications = await _jobApplicationRepository.GetAllAsync();

            var sourcesByEmail = new Dictionary<string, HashSet<ApplicationSourceEnum>>(StringComparer.OrdinalIgnoreCase);
            var resumeTextByEmail = new Dictionary<string, StringBuilder>(StringComparer.OrdinalIgnoreCase);

            foreach (var application in applications)
            {
                if (string.IsNullOrWhiteSpace(application.CandidateEmail))
                    continue;

                if (!sourcesByEmail.TryGetValue(application.CandidateEmail, out var sources))
                    sourcesByEmail[application.CandidateEmail] = sources = new HashSet<ApplicationSourceEnum>();
                sources.Add(application.Source);

                if (!string.IsNullOrWhiteSpace(application.ResumeExtractedText))
                {
                    if (!resumeTextByEmail.TryGetValue(application.CandidateEmail, out var sb))
                        resumeTextByEmail[application.CandidateEmail] = sb = new StringBuilder();
                    sb.Append(' ').Append(application.ResumeExtractedText);
                }
            }

            return (sourcesByEmail, resumeTextByEmail.ToDictionary(kv => kv.Key, kv => kv.Value.ToString(), StringComparer.OrdinalIgnoreCase));
        }

        private static CvBankSearchResultResponse ToResponse(CandidateProfile profile, CandidateFactService.CandidateFacts facts, int relevanceScore)
        {
            var topEducation = profile.Educations
                .OrderByDescending(e => e.EducationLevel)
                .FirstOrDefault();

            return new CvBankSearchResultResponse
            {
                CandidateProfileId = profile.CandidateProfileId,
                FullName = profile.FullName,
                Email = profile.Email,
                Phone = profile.Phone,
                ProfilePhotoPath = profile.ProfilePhotoPath,
                EducationSummary = topEducation != null ? $"{topEducation.DegreeTitle} - {topEducation.Institution}" : null,
                TotalExperienceYears = Math.Round(facts.TotalExperienceYears, 1),
                RelevanceScore = relevanceScore
            };
        }
    }
}
