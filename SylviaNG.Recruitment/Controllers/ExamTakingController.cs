using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ExamTaking.Commands.ExamTakingStart;
using SylviaNG.Recruitment.Application.Features.ExamTaking.Commands.ExamTakingSubmit;
using SylviaNG.Recruitment.Application.Features.ExamTaking.Models;
using SylviaNG.Recruitment.Application.Features.ExamTaking.Queries.ExamTakingGetMyEnrollments;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>Candidate-facing online exam attempt (US-058): list own enrollments, start,
    /// submit. Separate from ExamController, which is Admin/HR-only.</summary>
    [ApiController]
    [Route("recruitment/exam-taking")]
    [Authorize(Roles = "Candidate")]
    public class ExamTakingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExamTakingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>The current candidate's own exam enrollments, across all their applications (AC1).</summary>
        [HttpGet("enrollments")]
        public async Task<ActionResult<List<MyExamEnrollmentResponse>>> GetMyEnrollments()
        {
            var result = await _mediator.Send(new ExamTakingGetMyEnrollmentsQuery());
            return Ok(result);
        }

        /// <summary>Starts (or resumes) an online exam attempt, returning the paper without an answer key (AC2).</summary>
        [HttpPost("enrollments/{examEnrollmentId}/start")]
        public async Task<ActionResult<ExamPaperResponse>> Start(long examEnrollmentId)
        {
            var result = await _mediator.Send(new ExamTakingStartCommand(examEnrollmentId));
            return Ok(result);
        }

        /// <summary>Final submission - auto-scores objective questions, flags Subjective for manual review (AC3/AC4/AC5).</summary>
        [HttpPost("enrollments/{examEnrollmentId}/submit")]
        public async Task<ActionResult<ExamSubmitResultResponse>> Submit(long examEnrollmentId, [FromBody] ExamSubmitRequest request)
        {
            var result = await _mediator.Send(new ExamTakingSubmitCommand(examEnrollmentId, request));
            return Ok(result);
        }
    }
}
