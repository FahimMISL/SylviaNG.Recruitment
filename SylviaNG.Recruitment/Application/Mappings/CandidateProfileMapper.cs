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
            entity.BloodGroup = request.BloodGroup;
        }

        public static void ApplyContactUpdate(this CandidateProfile entity, CandidateProfileContactUpdateRequest request)
        {
            entity.Email = request.Email;
            entity.Phone = request.Phone;
            entity.MobileOperator = request.MobileOperator;
            entity.PresentDivisionId = request.PresentDivisionId;
            entity.PresentDistrictId = request.PresentDistrictId;
            entity.PresentThanaId = request.PresentThanaId;
            entity.PresentAddressDetail = request.PresentAddressDetail;
            entity.HomeDivisionId = request.HomeDivisionId;
            entity.HomeDistrictId = request.HomeDistrictId;
            entity.HomeThanaId = request.HomeThanaId;
            entity.PermanentAddressDetail = request.PermanentAddressDetail;
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
                BloodGroup = entity.BloodGroup,
                Email = entity.Email,
                Phone = entity.Phone,
                MobileOperator = entity.MobileOperator,
                PresentDivisionId = entity.PresentDivisionId,
                PresentDistrictId = entity.PresentDistrictId,
                PresentThanaId = entity.PresentThanaId,
                PresentAddressDetail = entity.PresentAddressDetail,
                HomeDivisionId = entity.HomeDivisionId,
                HomeDistrictId = entity.HomeDistrictId,
                HomeThanaId = entity.HomeThanaId,
                PermanentAddressDetail = entity.PermanentAddressDetail,
                ProfilePhotoPath = entity.ProfilePhotoPath,
                SignaturePath = entity.SignaturePath,
                CompletenessPercentage = CalculateCompleteness(entity),
                IsInternal = entity.IsInternal,
                HasPrepopulatedFieldEdits = HasPrepopulatedFieldEdits(entity)
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
                CompletenessPercentage = CalculateCompleteness(entity),
                IsInternal = entity.IsInternal
            };
        }

        public static CandidateProfileDetailResponse ToDetailResponse(
            this CandidateProfile entity,
            List<JobApplication> applications,
            List<TalentPoolCandidate> poolMemberships)
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
                BloodGroup = entity.BloodGroup,
                Email = entity.Email,
                Phone = entity.Phone,
                MobileOperator = entity.MobileOperator,
                PresentDivisionId = entity.PresentDivisionId,
                PresentDistrictId = entity.PresentDistrictId,
                PresentThanaId = entity.PresentThanaId,
                PresentAddressDetail = entity.PresentAddressDetail,
                HomeDivisionId = entity.HomeDivisionId,
                HomeDistrictId = entity.HomeDistrictId,
                HomeThanaId = entity.HomeThanaId,
                PermanentAddressDetail = entity.PermanentAddressDetail,
                ProfilePhotoPath = entity.ProfilePhotoPath,
                SignaturePath = entity.SignaturePath,
                CompletenessPercentage = CalculateCompleteness(entity),
                IsInternal = entity.IsInternal,
                HasPrepopulatedFieldEdits = HasPrepopulatedFieldEdits(entity),
                Educations = entity.Educations.Select(e => e.ToResponse()).ToList(),
                WorkExperiences = entity.WorkExperiences.Select(e => e.ToResponse()).ToList(),
                Skills = entity.Skills.Select(e => e.ToResponse()).ToList(),
                Certifications = entity.Certifications.Select(e => e.ToResponse()).ToList(),
                Documents = entity.Documents.Select(e => e.ToResponse()).ToList(),
                ApplicationHistory = applications.Select(a => a.ToResponse()).ToList(),
                HrNotes = entity.HrNotes,
                TalentPools = poolMemberships.Select(m => m.ToBadgeResponse()).ToList(),
                Tags = entity.Tags.Select(t => t.TagName).ToList()
            };
        }

        /// <summary>Public so JobApplicationService can re-run the same calculation for the
        /// US-007 AC4 minimum-completeness submit gate.</summary>
        public static int CalculateCompleteness(CandidateProfile entity)
        {
            var completedSections = 0;

            // Gated on each section's actual required form field (FullName / Email), not
            // incidental optional ones - otherwise a fully-saved section can still read as 0%.
            if (!string.IsNullOrWhiteSpace(entity.FullName))
                completedSections++; // Personal Info

            if (!string.IsNullOrWhiteSpace(entity.Email))
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

        // US-005 AC2: an internal candidate's pre-populated FullName/Phone were snapshotted at
        // provisioning time; if the live value has since diverged, flag it for HR. Always false
        // for external candidates (Prepopulated* stay null for them).
        private static bool HasPrepopulatedFieldEdits(CandidateProfile entity)
        {
            if (!entity.EmployeeId.HasValue)
                return false;

            var nameChanged = entity.PrepopulatedFullName != null && entity.FullName != entity.PrepopulatedFullName;
            var phoneChanged = entity.PrepopulatedPhone != null && entity.Phone != entity.PrepopulatedPhone;
            return nameChanged || phoneChanged;
        }

        // ── CandidateEducation Mappings ───────────────────────────────────

        public static CandidateEducation ToEntity(this CandidateEducationCreateRequest request, long candidateProfileId)
        {
            return new CandidateEducation
            {
                CandidateProfileId = candidateProfileId,
                DegreeTitle = request.DegreeTitle,
                Institution = request.Institution,
                EducationLevel = request.EducationLevel,
                PassingYear = request.PassingYear,
                Result = request.Result,
                MajorSubject = request.MajorSubject
            };
        }

        public static void ApplyUpdate(this CandidateEducation entity, CandidateEducationUpdateRequest request)
        {
            entity.DegreeTitle = request.DegreeTitle;
            entity.Institution = request.Institution;
            entity.EducationLevel = request.EducationLevel;
            entity.PassingYear = request.PassingYear;
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
                EducationLevel = entity.EducationLevel,
                PassingYear = entity.PassingYear,
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

        // ── CandidateTag Mappings (US-041, HR-only) ─────────────────────────

        public static CandidateTagResponse ToResponse(this CandidateTag entity)
        {
            return new CandidateTagResponse
            {
                CandidateTagId = entity.CandidateTagId,
                TagName = entity.TagName
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
