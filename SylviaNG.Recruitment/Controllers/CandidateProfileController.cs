using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateCertificationCreate;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateCertificationDelete;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateCertificationUpdate;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateDocumentDelete;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateDocumentUpdate;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateDocumentUpload;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateEducationCreate;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateEducationDelete;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateEducationUpdate;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfileContactUpdate;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfileHrNotesUpdate;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfilePersonalInfoUpdate;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfilePhotoDelete;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfilePhotoUpload;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfileSignatureDelete;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfileSignatureUpload;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateResumeParse;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateSkillCreate;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateSkillDelete;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateTagCreate;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateTagDelete;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateWorkExperienceCreate;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateWorkExperienceDelete;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateWorkExperienceUpdate;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateCertificationGetAll;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateDocumentGetAll;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateEducationGetAll;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateProfileGetById;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateProfileGetMe;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateProfileGetPaged;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateSkillGetAll;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateTagGetAll;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateTagSuggestions;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateWorkExperienceGetAll;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Controllers
{
    // No class-level [Authorize] - the global AuthorizeFilter already requires login for every
    // controller (see JobApplicationController for the established precedent). Any authenticated
    // role may hit this; Admin/HR just get an auto-provisioned, unused profile row.
    [ApiController]
    [Route("recruitment/candidate-profile")]
    public class CandidateProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CandidateProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get the current user's candidate profile, auto-provisioning one if it doesn't exist yet.
        /// </summary>
        [HttpGet("me")]
        public async Task<ActionResult<CandidateProfileResponse>> GetMe()
        {
            var result = await _mediator.Send(new CandidateProfileGetMeQuery());
            return Ok(result);
        }

        /// <summary>
        /// Update the current user's personal info section.
        /// </summary>
        [HttpPut("me/personal-info")]
        public async Task<ActionResult> UpdatePersonalInfo([FromBody] CandidateProfilePersonalInfoUpdateRequest request)
        {
            await _mediator.Send(new CandidateProfilePersonalInfoUpdateCommand(request));
            return Ok();
        }

        /// <summary>
        /// Update the current user's contact section.
        /// </summary>
        [HttpPut("me/contact")]
        public async Task<ActionResult> UpdateContact([FromBody] CandidateProfileContactUpdateRequest request)
        {
            await _mediator.Send(new CandidateProfileContactUpdateCommand(request));
            return Ok();
        }

        // ── Education ──────────────────────────────────────────────────

        [HttpGet("me/education")]
        public async Task<ActionResult<List<CandidateEducationResponse>>> GetEducation()
        {
            var result = await _mediator.Send(new CandidateEducationGetAllQuery());
            return Ok(result);
        }

        [HttpPost("me/education")]
        public async Task<ActionResult<long>> CreateEducation([FromBody] CandidateEducationCreateRequest request)
        {
            var id = await _mediator.Send(new CandidateEducationCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("me/education/{candidateEducationId}")]
        public async Task<ActionResult> UpdateEducation(long candidateEducationId, [FromBody] CandidateEducationUpdateRequest request)
        {
            await _mediator.Send(new CandidateEducationUpdateCommand(candidateEducationId, request));
            return Ok();
        }

        [HttpDelete("me/education/{candidateEducationId}")]
        public async Task<ActionResult> DeleteEducation(long candidateEducationId)
        {
            await _mediator.Send(new CandidateEducationDeleteCommand(candidateEducationId));
            return Ok();
        }

        // ── Work Experience ────────────────────────────────────────────

        [HttpGet("me/work-experience")]
        public async Task<ActionResult<List<CandidateWorkExperienceResponse>>> GetWorkExperience()
        {
            var result = await _mediator.Send(new CandidateWorkExperienceGetAllQuery());
            return Ok(result);
        }

        [HttpPost("me/work-experience")]
        public async Task<ActionResult<long>> CreateWorkExperience([FromBody] CandidateWorkExperienceCreateRequest request)
        {
            var id = await _mediator.Send(new CandidateWorkExperienceCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("me/work-experience/{candidateWorkExperienceId}")]
        public async Task<ActionResult> UpdateWorkExperience(long candidateWorkExperienceId, [FromBody] CandidateWorkExperienceUpdateRequest request)
        {
            await _mediator.Send(new CandidateWorkExperienceUpdateCommand(candidateWorkExperienceId, request));
            return Ok();
        }

        [HttpDelete("me/work-experience/{candidateWorkExperienceId}")]
        public async Task<ActionResult> DeleteWorkExperience(long candidateWorkExperienceId)
        {
            await _mediator.Send(new CandidateWorkExperienceDeleteCommand(candidateWorkExperienceId));
            return Ok();
        }

        // ── Skills (add/remove only — no PUT, matches plan API surface) ────

        [HttpGet("me/skills")]
        public async Task<ActionResult<List<CandidateSkillResponse>>> GetSkills()
        {
            var result = await _mediator.Send(new CandidateSkillGetAllQuery());
            return Ok(result);
        }

        [HttpPost("me/skills")]
        public async Task<ActionResult<long>> CreateSkill([FromBody] CandidateSkillCreateRequest request)
        {
            var id = await _mediator.Send(new CandidateSkillCreateCommand(request));
            return Ok(id);
        }

        [HttpDelete("me/skills/{candidateSkillId}")]
        public async Task<ActionResult> DeleteSkill(long candidateSkillId)
        {
            await _mediator.Send(new CandidateSkillDeleteCommand(candidateSkillId));
            return Ok();
        }

        // ── Certifications (multipart: scalar fields + optional certificate file) ──

        [HttpGet("me/certifications")]
        public async Task<ActionResult<List<CandidateCertificationResponse>>> GetCertifications()
        {
            var result = await _mediator.Send(new CandidateCertificationGetAllQuery());
            return Ok(result);
        }

        [HttpPost("me/certifications")]
        public async Task<ActionResult<long>> CreateCertification([FromForm] CandidateCertificationCreateRequest request)
        {
            var id = await _mediator.Send(new CandidateCertificationCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("me/certifications/{candidateCertificationId}")]
        public async Task<ActionResult> UpdateCertification(long candidateCertificationId, [FromForm] CandidateCertificationUpdateRequest request)
        {
            await _mediator.Send(new CandidateCertificationUpdateCommand(candidateCertificationId, request));
            return Ok();
        }

        [HttpDelete("me/certifications/{candidateCertificationId}")]
        public async Task<ActionResult> DeleteCertification(long candidateCertificationId)
        {
            await _mediator.Send(new CandidateCertificationDeleteCommand(candidateCertificationId));
            return Ok();
        }

        // ── Photo ──────────────────────────────────────────────────────

        [HttpPost("me/photo")]
        public async Task<ActionResult<string>> UploadPhoto([FromForm] IFormFile file)
        {
            var path = await _mediator.Send(new CandidateProfilePhotoUploadCommand(file));
            return Ok(path);
        }

        [HttpDelete("me/photo")]
        public async Task<ActionResult> DeletePhoto()
        {
            await _mediator.Send(new CandidateProfilePhotoDeleteCommand());
            return Ok();
        }

        // ── Signature ──────────────────────────────────────────────────

        [HttpPost("me/signature")]
        public async Task<ActionResult<string>> UploadSignature([FromForm] IFormFile file)
        {
            var path = await _mediator.Send(new CandidateProfileSignatureUploadCommand(file));
            return Ok(path);
        }

        [HttpDelete("me/signature")]
        public async Task<ActionResult> DeleteSignature()
        {
            await _mediator.Send(new CandidateProfileSignatureDeleteCommand());
            return Ok();
        }

        // ── Documents (multipart, categorized by DocumentType) ──────────

        [HttpGet("me/documents")]
        public async Task<ActionResult<List<CandidateDocumentResponse>>> GetDocuments()
        {
            var result = await _mediator.Send(new CandidateDocumentGetAllQuery());
            return Ok(result);
        }

        [HttpPost("me/documents")]
        public async Task<ActionResult<CandidateDocumentResponse>> UploadDocument([FromForm] CandidateDocumentUploadRequest request)
        {
            var result = await _mediator.Send(new CandidateDocumentUploadCommand(request));
            return Ok(result);
        }

        [HttpPut("me/documents/{candidateDocumentId}")]
        public async Task<ActionResult<CandidateDocumentResponse>> UpdateDocument(long candidateDocumentId, [FromForm] CandidateDocumentUpdateRequest request)
        {
            var result = await _mediator.Send(new CandidateDocumentUpdateCommand(candidateDocumentId, request));
            return Ok(result);
        }

        [HttpDelete("me/documents/{candidateDocumentId}")]
        public async Task<ActionResult> DeleteDocument(long candidateDocumentId)
        {
            await _mediator.Send(new CandidateDocumentDeleteCommand(candidateDocumentId));
            return Ok();
        }

        // ── Resume parse (Heuristic or Ai provider, see ResumeParsing:Provider) ──

        /// <summary>
        /// Parses an uploaded resume (PDF/DOCX) and returns best-effort extracted fields for the
        /// frontend to prefill section forms with - the candidate still reviews and hits Save per
        /// section as usual. The file itself is also saved as a Resume CandidateDocument in the
        /// same request (see CandidateResumeParseResponse.ResumeDocumentSaved).
        /// </summary>
        [HttpPost("me/resume-parse")]
        public async Task<ActionResult<CandidateResumeParseResponse>> ParseResume([FromForm] IFormFile file)
        {
            var result = await _mediator.Send(new CandidateResumeParseCommand(file));
            return Ok(result);
        }

        // ── HR/Admin read-only candidate view (US-009) ────────────────────

        /// <summary>
        /// Paged, searchable list of candidate profiles for HR/Admin to browse.
        /// </summary>
        [HttpGet("paged")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<PagedResult<CandidateProfileSummaryResponse>>> GetPaged(
            [FromQuery] PagedRequest request,
            [FromQuery] List<long>? talentPoolIds = null,
            [FromQuery] List<string>? tags = null)
        {
            var result = await _mediator.Send(new CandidateProfileGetPagedQuery(request, talentPoolIds, tags));
            return Ok(result);
        }

        /// <summary>
        /// Full read-only aggregate of one candidate's profile (all sections, completeness,
        /// application history, HR notes) for HR/Admin.
        /// </summary>
        [HttpGet("{candidateProfileId:long}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<CandidateProfileDetailResponse>> GetById(long candidateProfileId)
        {
            var result = await _mediator.Send(new CandidateProfileGetByIdQuery(candidateProfileId));
            return Ok(result);
        }

        /// <summary>
        /// Updates the HR-only annotation on a candidate's profile. Never visible to or editable
        /// by the candidate.
        /// </summary>
        [HttpPut("{candidateProfileId:long}/hr-notes")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult> UpdateHrNotes(long candidateProfileId, [FromBody] CandidateProfileHrNotesUpdateRequest request)
        {
            await _mediator.Send(new CandidateProfileHrNotesUpdateCommand(candidateProfileId, request.HrNotes));
            return Ok();
        }

        // ── Tags (US-041, HR/Admin-only - never visible to the candidate) ──

        /// <summary>
        /// Distinct tag names already used across candidates, for the add-tag autocomplete and
        /// the candidate-list/ATS dashboard tag filter's suggestion source (AC2).
        /// </summary>
        [HttpGet("tags/suggestions")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<List<string>>> GetTagSuggestions([FromQuery] string? search)
        {
            var result = await _mediator.Send(new CandidateTagSuggestionsQuery(search));
            return Ok(result);
        }

        [HttpGet("{candidateProfileId:long}/tags")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<List<CandidateTagResponse>>> GetTags(long candidateProfileId)
        {
            var result = await _mediator.Send(new CandidateTagGetAllQuery(candidateProfileId));
            return Ok(result);
        }

        [HttpPost("{candidateProfileId:long}/tags")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<long>> CreateTag(long candidateProfileId, [FromBody] CandidateTagCreateRequest request)
        {
            var id = await _mediator.Send(new CandidateTagCreateCommand(candidateProfileId, request));
            return Ok(id);
        }

        [HttpDelete("{candidateProfileId:long}/tags/{candidateTagId:long}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult> DeleteTag(long candidateProfileId, long candidateTagId)
        {
            await _mediator.Send(new CandidateTagDeleteCommand(candidateProfileId, candidateTagId));
            return Ok();
        }
    }
}
