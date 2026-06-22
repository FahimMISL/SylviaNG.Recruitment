using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class CandidateCertificationMapper
    {
        public static CandidateCertification ToEntity(this CandidateCertificationCreateRequest request)
        {
            return new CandidateCertification
            {
                CandidateId = request.CandidateId,
                CertificationName = request.CertificationName,
                IssuingOrganization = request.IssuingOrganization,
                IssueDate = request.IssueDate,
                ExpiryDate = request.ExpiryDate,
                CredentialId = request.CredentialId,
                CredentialUrl = request.CredentialUrl,
            };
        }

        public static void ApplyUpdate(this CandidateCertification entity, CandidateCertificationUpdateRequest request)
        {
            if (request.CandidateId.HasValue) entity.CandidateId = request.CandidateId.Value;
            if (request.CertificationName is not null) entity.CertificationName = request.CertificationName;
            if (request.IssuingOrganization is not null) entity.IssuingOrganization = request.IssuingOrganization;
            if (request.IssueDate.HasValue) entity.IssueDate = request.IssueDate;
            if (request.ExpiryDate.HasValue) entity.ExpiryDate = request.ExpiryDate;
            if (request.CredentialId is not null) entity.CredentialId = request.CredentialId;
            if (request.CredentialUrl is not null) entity.CredentialUrl = request.CredentialUrl;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static CandidateCertificationResponse ToResponse(this CandidateCertification entity)
        {
            return new CandidateCertificationResponse
            {
                CandidateCertificationId = entity.CandidateCertificationId,
                CandidateId = entity.CandidateId,
                CertificationName = entity.CertificationName,
                IssuingOrganization = entity.IssuingOrganization,
                IssueDate = entity.IssueDate,
                ExpiryDate = entity.ExpiryDate,
                CredentialId = entity.CredentialId,
                CredentialUrl = entity.CredentialUrl,
                IsActive = entity.IsActive,
            };
        }
    }
}
