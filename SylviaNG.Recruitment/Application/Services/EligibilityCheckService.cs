using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Services
{
    public class EligibilityCheckService : IEligibilityCheckService
    {
        public (bool IsEligible, List<string> Reasons) CheckEligibility(Candidate candidate, JobPosting job)
        {
            var reasons = new List<string>();

            if (job.MinAge.HasValue || job.MaxAge.HasValue)
            {
                var age = CalculateAge(candidate.DateOfBirth);
                if (age.HasValue)
                {
                    if (job.MinAge.HasValue && age.Value < job.MinAge.Value)
                        reasons.Add($"Minimum age requirement is {job.MinAge} years (candidate is {age}).");
                    if (job.MaxAge.HasValue && age.Value > job.MaxAge.Value)
                        reasons.Add($"Maximum age limit is {job.MaxAge} years (candidate is {age}).");
                }
                else if (job.MinAge.HasValue || job.MaxAge.HasValue)
                {
                    reasons.Add("Date of birth is required to verify age eligibility.");
                }
            }

            if (job.MinExperienceYears.HasValue)
            {
                var experience = candidate.TotalExperienceYears ?? 0;
                if (experience < job.MinExperienceYears.Value)
                    reasons.Add($"Minimum {job.MinExperienceYears} years of experience required (candidate has {experience}).");
            }

            if (job.MinEducationLevel.HasValue)
            {
                var candidateLevel = GetHighestEducationLevel(candidate);
                if (candidateLevel == null || candidateLevel.Value < job.MinEducationLevel.Value)
                {
                    reasons.Add($"Minimum education level is {job.MinEducationLevel.Value}.");
                }
            }

            if (!string.IsNullOrWhiteSpace(job.RequiredDistrict))
            {
                var candidateDistrict = candidate.PermanentAddress;
                if (string.IsNullOrWhiteSpace(candidateDistrict) ||
                    !candidateDistrict.Contains(job.RequiredDistrict, StringComparison.OrdinalIgnoreCase))
                {
                    reasons.Add($"This position requires candidates from {job.RequiredDistrict} district.");
                }
            }

            return (reasons.Count == 0, reasons);
        }

        private static int? CalculateAge(DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue) return null;
            var today = DateTime.UtcNow;
            var age = today.Year - dateOfBirth.Value.Year;
            if (dateOfBirth.Value.Date > today.AddYears(-age)) age--;
            return age;
        }

        private static EducationLevelEnum? GetHighestEducationLevel(Candidate candidate)
        {
            if (candidate.Educations == null || !candidate.Educations.Any())
                return null;

            var levelMap = new Dictionary<string, EducationLevelEnum>(StringComparer.OrdinalIgnoreCase)
            {
                ["ssc"] = EducationLevelEnum.SSC,
                ["secondary"] = EducationLevelEnum.SSC,
                ["hsc"] = EducationLevelEnum.HSC,
                ["higher secondary"] = EducationLevelEnum.HSC,
                ["intermediate"] = EducationLevelEnum.HSC,
                ["diploma"] = EducationLevelEnum.Diploma,
                ["bachelor"] = EducationLevelEnum.Bachelors,
                ["bsc"] = EducationLevelEnum.Bachelors,
                ["bba"] = EducationLevelEnum.Bachelors,
                ["bs"] = EducationLevelEnum.Bachelors,
                ["master"] = EducationLevelEnum.Masters,
                ["msc"] = EducationLevelEnum.Masters,
                ["mba"] = EducationLevelEnum.Masters,
                ["ms"] = EducationLevelEnum.Masters,
                ["phd"] = EducationLevelEnum.PhD,
                ["doctorate"] = EducationLevelEnum.PhD,
            };

            EducationLevelEnum? highest = null;

            foreach (var edu in candidate.Educations.Where(e => e.IsActive))
            {
                var degree = edu.Degree?.ToLowerInvariant() ?? "";
                foreach (var kvp in levelMap)
                {
                    if (degree.Contains(kvp.Key) && (!highest.HasValue || kvp.Value > highest.Value))
                    {
                        highest = kvp.Value;
                    }
                }
            }

            return highest;
        }
    }
}
