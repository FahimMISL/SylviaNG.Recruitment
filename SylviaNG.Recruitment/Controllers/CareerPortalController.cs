using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobApplicationSubmit;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetAllPublicPaged;
using SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetPublicById;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>
    /// Public career portal: anonymous browsing of externally-visible job postings, but applying
    /// requires a logged-in Candidate account - no guest apply. Browse actions are individually
    /// [AllowAnonymous] to opt out of the global RequireAuthenticatedUser() MVC filter registered
    /// in Program.cs; Apply is deliberately left off that list so it falls back to that filter,
    /// narrowed further to the Candidate role.
    /// </summary>
    [ApiController]
    [Route("recruitment/career-portal")]
    public class CareerPortalController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly ManualShortlistScoringService _scoringService;

        public CareerPortalController(
            IMediator mediator,
            ICurrentCandidateService currentCandidateService,
            ICandidateProfileRepository candidateProfileRepository,
            IJobPostingRepository jobPostingRepository,
            ManualShortlistScoringService scoringService)
        {
            _mediator = mediator;
            _currentCandidateService = currentCandidateService;
            _candidateProfileRepository = candidateProfileRepository;
            _jobPostingRepository = jobPostingRepository;
            _scoringService = scoringService;
        }

        /// <summary>
        /// Browse paginated, published (Open, CircularType ExternalOnly/Both) job postings.
        /// </summary>
        [HttpGet("job-postings")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<JobPostingResponse>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] string? location,
            [FromQuery] long? departmentId,
            [FromQuery] EmploymentTypeEnum? employmentType,
            [FromQuery] int? maxExperienceYears)
        {
            var result = await _mediator.Send(new JobPostingGetAllPublicPagedQuery(request, location, departmentId, employmentType, maxExperienceYears));
            return Ok(result);
        }

        /// <summary>
        /// Get a single published job posting's detail view.
        /// </summary>
        [HttpGet("job-postings/{jobPostingId}")]
        [AllowAnonymous]
        public async Task<ActionResult<JobPostingResponse>> GetById(long jobPostingId)
        {
            var result = await _mediator.Send(new JobPostingGetPublicByIdQuery(jobPostingId));
            return Ok(result);
        }

        /// <summary>
        /// Submit an application (with optional CV upload) to a published job posting. Requires a
        /// logged-in Candidate account - no guest apply.
        /// </summary>
        [HttpPost("job-postings/{jobPostingId}/apply")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<JobApplicationResponse>> Apply(long jobPostingId, [FromForm] JobApplicationSubmitRequest request)
        {
            request.JobPostingId = jobPostingId;
            var result = await _mediator.Send(new JobApplicationSubmitCommand(request, ApplicationSourceEnum.External));
            return Ok(result);
        }

        /// <summary>
        /// Get the profile-match score (0-100) for a single job posting against the
        /// logged-in candidate's profile. Only available to authenticated Candidate users.
        /// </summary>
        [HttpGet("job-postings/{jobPostingId}/match-score")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<object>> GetMatchScore(long jobPostingId)
        {
            var subjectId = _currentCandidateService.GetCurrentKeycloakSubjectId();
            var profile = await _candidateProfileRepository.GetByKeycloakSubjectIdWithDetailsAsync(subjectId);
            if (profile == null)
                return Ok(new { score = (int?)null });

            var posting = await _jobPostingRepository.GetOpenByIdAndCircularTypesAsync(
                jobPostingId,
                new[] { CircularTypeEnum.Both, CircularTypeEnum.ExternalOnly },
                ignoreCompanyScope: true);

            if (posting == null)
                return NotFound();

            var facts = CandidateFactService.BuildFacts(profile);
            var result = await _scoringService.ScoreAsync(posting, facts);

            return Ok(new { score = result.Score });
        }
    }
}
