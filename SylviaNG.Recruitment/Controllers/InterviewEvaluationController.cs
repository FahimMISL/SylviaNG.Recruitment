using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationSubmit;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationUpdate;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Queries.InterviewEvaluationGetById;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Queries.InterviewEvaluationGetByInterview;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Queries.InterviewEvaluationResultsExportExcel;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>Panelist scorecard evaluations of an Interview, entered by HR on the panelist's
    /// behalf (EP-08 US-067/US-068/US-071).</summary>
    [ApiController]
    [Route("recruitment/interview/{interviewId}/evaluation")]
    [Authorize(Roles = "Admin,HR")]
    public class InterviewEvaluationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InterviewEvaluationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>All panelist evaluations submitted for this interview.</summary>
        [HttpGet]
        public async Task<ActionResult<List<InterviewEvaluationResponse>>> GetByInterview(long interviewId)
        {
            var result = await _mediator.Send(new InterviewEvaluationGetByInterviewQuery(interviewId));
            return Ok(result);
        }

        /// <summary>Get a single evaluation by ID.</summary>
        [HttpGet("{evaluationId}")]
        public async Task<ActionResult<InterviewEvaluationResponse>> GetById(long interviewId, long evaluationId)
        {
            var result = await _mediator.Send(new InterviewEvaluationGetByIdQuery(evaluationId));
            return Ok(result);
        }

        /// <summary>Submit a panelist's evaluation (HR enters it on the panelist's behalf).</summary>
        [HttpPost]
        public async Task<ActionResult<long>> Submit(long interviewId, [FromBody] InterviewEvaluationSubmitRequest request)
        {
            var id = await _mediator.Send(new InterviewEvaluationSubmitCommand(interviewId, request));
            return Ok(id);
        }

        /// <summary>Correct a previously submitted evaluation.</summary>
        [HttpPut("{evaluationId}")]
        public async Task<ActionResult> Update(long interviewId, long evaluationId, [FromBody] InterviewEvaluationUpdateRequest request)
        {
            await _mediator.Send(new InterviewEvaluationUpdateCommand(evaluationId, request));
            return Ok();
        }

        /// <summary>Downloadable XLSX of every panelist's scores + weighted totals for this interview.</summary>
        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportExcel(long interviewId)
        {
            var file = await _mediator.Send(new InterviewEvaluationResultsExportExcelQuery(interviewId));
            return File(file.Content, file.ContentType, file.FileName);
        }
    }
}
