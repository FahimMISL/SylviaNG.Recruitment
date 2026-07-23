using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewBulkCancel;
using SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewBulkReschedule;
using SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewBulkSchedule;
using SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewCancel;
using SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewReschedule;
using SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewSchedule;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;
using SylviaNG.Recruitment.Application.Features.Interviews.Queries.InterviewGetAllPaged;
using SylviaNG.Recruitment.Application.Features.Interviews.Queries.InterviewGetById;
using SylviaNG.Recruitment.Application.Features.Interviews.Queries.InterviewGetByJobApplication;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>Interview scheduling/rescheduling/cancellation, single and bulk (EP-08 US-063/US-064/US-065).</summary>
    [ApiController]
    [Route("recruitment/interview")]
    [Authorize(Roles = "Admin,HR")]
    public class InterviewController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InterviewController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Searched/filtered/paged interview list.</summary>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<InterviewResponse>>> GetPaged(
            [FromQuery] PagedRequest request,
            [FromQuery] long? jobPostingId,
            [FromQuery] InterviewStatusEnum? status,
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo)
        {
            var result = await _mediator.Send(new InterviewGetAllPagedQuery(request, jobPostingId, status, dateFrom, dateTo));
            return Ok(result);
        }

        /// <summary>Get an interview by ID.</summary>
        [HttpGet("{interviewId}")]
        public async Task<ActionResult<InterviewResponse>> GetById(long interviewId)
        {
            var result = await _mediator.Send(new InterviewGetByIdQuery(interviewId));
            return Ok(result);
        }

        /// <summary>Every interview scheduled for one job application - feeds the candidate's pipeline-progress tracker.</summary>
        [HttpGet("job-application/{jobApplicationId}")]
        public async Task<ActionResult<List<InterviewResponse>>> GetByJobApplication(long jobApplicationId)
        {
            var result = await _mediator.Send(new InterviewGetByJobApplicationQuery(jobApplicationId));
            return Ok(result);
        }

        /// <summary>Schedule a single interview.</summary>
        [HttpPost]
        public async Task<ActionResult<long>> Schedule([FromBody] InterviewScheduleRequest request)
        {
            var id = await _mediator.Send(new InterviewScheduleCommand(request));
            return Ok(id);
        }

        /// <summary>Schedule interviews for several candidates at once, staggered from a shared start time.</summary>
        [HttpPost("bulk")]
        public async Task<ActionResult<List<long>>> BulkSchedule([FromBody] InterviewBulkScheduleRequest request)
        {
            var ids = await _mediator.Send(new InterviewBulkScheduleCommand(request));
            return Ok(ids);
        }

        /// <summary>Reschedule a single interview to a new date/time (and optionally a new venue/room/meeting link).</summary>
        [HttpPatch("{interviewId}/reschedule")]
        public async Task<ActionResult> Reschedule(long interviewId, [FromBody] InterviewRescheduleRequest request)
        {
            await _mediator.Send(new InterviewRescheduleCommand(interviewId, request));
            return Ok();
        }

        /// <summary>Reschedule several interviews at once, staggered from a shared start time.</summary>
        [HttpPatch("bulk/reschedule")]
        public async Task<ActionResult> BulkReschedule([FromBody] InterviewBulkRescheduleRequest request)
        {
            await _mediator.Send(new InterviewBulkRescheduleCommand(request));
            return Ok();
        }

        /// <summary>Cancel a single interview.</summary>
        [HttpPatch("{interviewId}/cancel")]
        public async Task<ActionResult> Cancel(long interviewId, [FromBody] InterviewCancelRequest request)
        {
            await _mediator.Send(new InterviewCancelCommand(interviewId, request));
            return Ok();
        }

        /// <summary>Cancel several interviews at once.</summary>
        [HttpPatch("bulk/cancel")]
        public async Task<ActionResult> BulkCancel([FromBody] InterviewBulkCancelRequest request)
        {
            await _mediator.Send(new InterviewBulkCancelCommand(request));
            return Ok();
        }
    }
}
