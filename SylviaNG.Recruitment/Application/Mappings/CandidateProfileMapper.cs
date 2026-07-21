using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class CandidateProfileMapper
    {
        // US-002: 7 candidate-profile sections total (Family Info dropped per user decision).
        // Work Experience and Certifications are intentionally NOT required for 100% (per plan)
        // — a candidate legitimately may have neither yet — but they still count toward the
        // completed total when present, same as every other section.
        private const int TotalSections = 7;

        public static void ApplyPersonalInfoUpdate(this CandidateProfile entity, CandidateProfilePersonalInfoUpdateRequest request)
        {
            entity.FullName = request.FullName;
            entity.DateOfBirth = request.DateOfBirth;
            entity.Gender = request.Gender;
            entity.NationalId = request.NationalId;
            entity.FatherName = request.FatherName;
            entity.MotherName = request.MotherName;
            entity.MaritalStatus = request.MaritalStatus;
            entity.Religion = request.Religion;
            entity.Nationality = request.Nationality;
        }

        public static void ApplyContactUpdate(this CandidateProfile entity, CandidateProfileContactUpdateRequest request)
        {
            entity.Email = request.Email;
            entity.Phone = request.Phone;
            entity.PresentAddress = request.PresentAddress;
            entity.PermanentAddress = request.PermanentAddress;
        }

        public static CandidateProfileResponse ToResponse(this CandidateProfile entity)
        {
            return new CandidateProfileResponse
            {
                CandidateProfileId = entity.CandidateProfileId,
                FullName = entity.FullName,
                DateOfBirth = entity.DateOfBirth,
                Gender = entity.Gender,
                NationalId = entity.NationalId,
                FatherName = entity.FatherName,
                MotherName = entity.MotherName,
                MaritalStatus = entity.MaritalStatus,
                Religion = entity.Religion,
                Nationality = entity.Nationality,
                Email = entity.Email,
                Phone = entity.Phone,
                PresentAddress = entity.PresentAddress,
                PermanentAddress = entity.PermanentAddress,
                ProfilePhotoPath = entity.ProfilePhotoPath,
                SignaturePath = entity.SignaturePath,
                CompletenessPercentage = CalculateCompleteness(entity)
            };
        }

        public static CandidateProfileSummaryResponse ToSummaryResponse(this CandidateProfile entity)
        {
            return new CandidateProfileSummaryResponse
            {
                CandidateProfileId = entity.CandidateProfileId,
                FullName = entity.FullName,
                Email = entity.Email,
                Phone = entity.Phone,
                ProfilePhotoPath = entity.ProfilePhotoPath,
                CompletenessPercentage = CalculateCompleteness(entity)
            };
        }

        public static CandidateProfileDetailResponse ToDetailResponse(this CandidateProfile entity, List<JobApplication> applications)
        {
            return new CandidateProfileDetailResponse
            {
                CandidateProfileId = entity.CandidateProfileId,
                FullName = entity.FullName,
                DateOfBirth = entity.DateOfBirth,
                Gender = entity.Gender,
                NationalId = entity.NationalId,
                FatherName = entity.FatherName,
                MotherName = entity.MotherName,
                MaritalStatus = entity.MaritalStatus,
                Religion = entity.Religion,
                Nationality = entity.Nationality,
                Email = entity.Email,
                Phone = entity.Phone,
                PresentAddress = entity.PresentAddress,
                PermanentAddress = entity.PermanentAddress,
                ProfilePhotoPath = entity.ProfilePhotoPath,
                SignaturePath = entity.SignaturePath,
                CompletenessPercentage = CalculateCompleteness(entity),
                Educations = entity.Educations.Select(e => e.ToResponse()).ToList(),
                WorkExperiences = entity.WorkExperiences.Select(e => e.ToResponse()).ToList(),
                Skills = entity.Skills.Select(e => e.ToResponse()).ToList(),
                Certifications = entity.Certifications.Select(e => e.ToResponse()).ToList(),
                Documents = entity.Documents.Select(e => e.ToResponse()).ToList(),
                ApplicationHistory = applications.Select(a => a.ToResponse()).ToList(),
                HrNotes = entity.HrNotes
            };
        }

        private static int CalculateCompleteness(CandidateProfile entity)
        {
            var completedSections = 0;

            if (entity.DateOfBirth.HasValue && !string.IsNullOrWhiteSpace(entity.Gender))
                completedSections++; // Personal Info

            if (!string.IsNullOrWhiteSpace(entity.Phone) && !string.IsNullOrWhiteSpace(entity.PresentAddress))
                completedSections++; // Contact

            if (entity.Educations.Count > 0)
                completedSections++; // Education

            if (entity.WorkExperiences.Count > 0)
                completedSections++; // Work Experience

            if (entity.Skills.Count > 0)
                completedSections++; // Skills

            if (entity.Certifications.Count > 0)
                completedSections++; // Certifications

            if (entity.Documents.Count > 0)
                completedSections++; // Documents

            return completedSections * 100 / TotalSections;
        }

        // ── CandidateEducation Mappings ───────────────────────────────────

        public static CandidateEducation ToEntity(this CandidateEducationCreateRequest request, long candidateProfileId)
        {
            return new CandidateEducation
            {
                CandidateProfileId = candidateProfileId,
                DegreeTitle = request.DegreeTitle,
                Institution = request.Institution,
                UniversityLibraryItemId = request.UniversityLibraryItemId,
                EducationLevel = request.EducationLevel,
                PassingYear = request.PassingYear,
                GradingSystem = request.GradingSystem,
                Result = request.Result,
                MajorSubject = request.MajorSubject
            };
        }

        public static void ApplyUpdate(this CandidateEducation entity, CandidateEducationUpdateRequest request)
        {
            entity.DegreeTitle = request.DegreeTitle;
            entity.Institution = request.Institution;
            entity.UniversityLibraryItemId = request.UniversityLibraryItemId;
            entity.EducationLevel = request.EducationLevel;
            entity.PassingYear = request.PassingYear;
            entity.GradingSystem = request.GradingSystem;
            entity.Result = request.Result;
            entity.MajorSubject = request.MajorSubject;
        }

        public static CandidateEducationResponse ToResponse(this CandidateEducation entity)
        {
            return new CandidateEducationResponse
            {
                CandidateEducationId = entity.CandidateEducationId,
                DegreeTitle = entity.DegreeTitle,
                Institution = entity.Institution,
                UniversityLibraryItemId = entity.UniversityLibraryItemId,
                EducationLevel = entity.EducationLevel,
                PassingYear = entity.PassingYear,
                GradingSystem = entity.GradingSystem,
                Result = entity.Result,
                MajorSubject = entity.MajorSubject
            };
        }

        // ── CandidateWorkExperience Mappings ──────────────────────────────

        public static CandidateWorkExperience ToEntity(this CandidateWorkExperienceCreateRequest request, long candidateProfileId)
        {
            return new CandidateWorkExperience
            {
                CandidateProfileId = candidateProfileId,
                CompanyName = request.CompanyName,
                Designation = request.Designation,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsCurrent = request.IsCurrent,
                Responsibilities = request.Responsibilities,
                Location = request.Location
            };
        }

        public static void ApplyUpdate(this CandidateWorkExperience entity, CandidateWorkExperienceUpdateRequest request)
        {
            entity.CompanyName = request.CompanyName;
            entity.Designation = request.Designation;
            entity.StartDate = request.StartDate;
            entity.EndDate = request.EndDate;
            entity.IsCurrent = request.IsCurrent;
            entity.Responsibilities = request.Responsibilities;
            entity.Location = request.Location;
        }

        public static CandidateWorkExperienceResponse ToResponse(this CandidateWorkExperience entity)
        {
            return new CandidateWorkExperienceResponse
            {
                CandidateWorkExperienceId = entity.CandidateWorkExperienceId,
                CompanyName = entity.CompanyName,
                Designation = entity.Designation,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                IsCurrent = entity.IsCurrent,
                Responsibilities = entity.Responsibilities,
                Location = entity.Location
            };
        }

        // ── CandidateSkill Mappings ───────────────────────────────────────

        public static CandidateSkill ToEntity(this CandidateSkillCreateRequest request, long candidateProfileId)
        {
            return new CandidateSkill
            {
                CandidateProfileId = candidateProfileId,
                SkillName = request.SkillName,
                SkillLibraryItemId = request.SkillLibraryItemId,
                ProficiencyLevel = request.ProficiencyLevel
            };
        }

        public static CandidateSkillResponse ToResponse(this CandidateSkill entity)
        {
            return new CandidateSkillResponse
            {
                CandidateSkillId = entity.CandidateSkillId,
                SkillName = entity.SkillName,
                SkillLibraryItemId = entity.SkillLibraryItemId,
                ProficiencyLevel = entity.ProficiencyLevel
            };
        }

        // ── SkillLibraryItem Mappings ──────────────────────────────────────

        public static SkillLibraryItemResponse ToResponse(this SkillLibraryItem entity)
        {
            return new SkillLibraryItemResponse
            {
                SkillLibraryItemId = entity.SkillLibraryItemId,
                Name = entity.Name
            };
        }

        // ── CandidateCertification Mappings ───────────────────────────────

        public static CandidateCertification ToEntity(this CandidateCertificationCreateRequest request, long candidateProfileId)
        {
            return new CandidateCertification
            {
                CandidateProfileId = candidateProfileId,
                CertificationName = request.CertificationName,
                IssuingOrganization = request.IssuingOrganization,
                IssueDate = request.IssueDate,
                ExpiryDate = request.ExpiryDate,
                CredentialId = request.CredentialId
            };
        }

        public static void ApplyUpdate(this CandidateCertification entity, CandidateCertificationUpdateRequest request)
        {
            entity.CertificationName = request.CertificationName;
            entity.IssuingOrganization = request.IssuingOrganization;
            entity.IssueDate = request.IssueDate;
            entity.ExpiryDate = request.ExpiryDate;
            entity.CredentialId = request.CredentialId;
        }

        public static CandidateCertificationResponse ToResponse(this CandidateCertification entity)
        {
            return new CandidateCertificationResponse
            {
                CandidateCertificationId = entity.CandidateCertificationId,
                CertificationName = entity.CertificationName,
                IssuingOrganization = entity.IssuingOrganization,
                IssueDate = entity.IssueDate,
                ExpiryDate = entity.ExpiryDate,
                CredentialId = entity.CredentialId,
                CertificateFilePath = entity.CertificateFilePath
            };
        }

        // ── CandidateDocument Mappings ─────────────────────────────────────

        public static CandidateDocumentResponse ToResponse(this CandidateDocument entity)
        {
            return new CandidateDocumentResponse
            {
                CandidateDocumentId = entity.CandidateDocumentId,
                DocumentType = entity.DocumentType,
                FileName = entity.FileName,
                ContentType = entity.ContentType,
                FileSizeBytes = entity.FileSizeBytes,
                IsActive = entity.IsActive,
                DownloadUrl = "/" + entity.FilePath.TrimStart('/')
            };
        }
    }
}
