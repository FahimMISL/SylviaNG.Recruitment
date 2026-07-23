using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Exams.Commands.ExamCreate;
using SylviaNG.Recruitment.Application.Features.Exams.Models;
using SylviaNG.Recruitment.Application.Features.Exams.Queries.ExamGetAllPaged;
using SylviaNG.Recruitment.Application.Features.Exams.Queries.ExamGetById;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamEnrollCandidates;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamEnrollmentReassignSeat;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamSeatPlanGenerate;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamAdmitCardDownloadPdf;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamEnrollmentGetByExam;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamSeatPlanDownloadExcel;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamSeatPlanDownloadPdf;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>Exam scheduling, candidate enrollment, and seat-plan management (US-055/US-056).</summary>
    [ApiController]
    [Route("recruitment/exam")]
    [Authorize(Roles = "Admin,HR")]
    public class ExamController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExamController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Searched/filtered/paged exam list.</summary>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ExamResponse>>> GetPaged(
            [FromQuery] PagedRequest request,
            [FromQuery] long? jobPostingId,
            [FromQuery] ExamTypeEnum? examType,
            [FromQuery] bool? isActive)
        {
            var result = await _mediator.Send(new ExamGetAllPagedQuery(request, jobPostingId, examType, isActive));
            return Ok(result);
        }

        /// <summary>Get an exam by ID.</summary>
        [HttpGet("{examId}")]
        public async Task<ActionResult<ExamResponse>> GetById(long examId)
        {
            var result = await _mediator.Send(new ExamGetByIdQuery(examId));
            return Ok(result);
        }

        /// <summary>Schedule a new exam.</summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ExamCreateRequest request)
        {
            var id = await _mediator.Send(new ExamCreateCommand(request));
            return Ok(id);
        }

        /// <summary>Enroll one or more shortlisted candidates (job applications) into this exam.</summary>
        [HttpPost("{examId}/enroll")]
        public async Task<ActionResult<List<long>>> Enroll(long examId, [FromBody] List<long> jobApplicationIds)
        {
            var result = await _mediator.Send(new ExamEnrollCandidatesCommand(examId, jobApplicationIds));
            return Ok(result);
        }

        /// <summary>Full enrollment roster for this exam.</summary>
        [HttpGet("{examId}/enrollments")]
        public async Task<ActionResult<List<ExamEnrollmentResponse>>> GetEnrollments(long examId)
        {
            var result = await _mediator.Send(new ExamEnrollmentGetByExamQuery(examId));
            return Ok(result);
        }

        /// <summary>Generate (or fully regenerate) the room/seat assignment for every enrollment in this exam.</summary>
        [HttpPost("{examId}/seat-plan/generate")]
        public async Task<ActionResult> GenerateSeatPlan(long examId)
        {
            await _mediator.Send(new ExamSeatPlanGenerateCommand(examId));
            return Ok();
        }

        /// <summary>Manually reassign a single enrollment to a different room/seat.</summary>
        [HttpPatch("{examId}/enrollments/{enrollmentId}/seat")]
        public async Task<ActionResult> ReassignSeat(long examId, long enrollmentId, [FromBody] ExamEnrollmentReassignSeatRequest request)
        {
            await _mediator.Send(new ExamEnrollmentReassignSeatCommand(enrollmentId, request.ExamRoomId, request.SeatNumber));
            return Ok();
        }

        /// <summary>Downloadable PDF of the full seat plan, grouped by room.</summary>
        [HttpGet("{examId}/seat-plan/download/pdf")]
        public async Task<IActionResult> DownloadSeatPlanPdf(long examId)
        {
            var file = await _mediator.Send(new ExamSeatPlanDownloadPdfQuery(examId));
            return File(file.Content, file.ContentType, file.FileName);
        }

        /// <summary>Downloadable Excel of the full seat plan, one row per candidate.</summary>
        [HttpGet("{examId}/seat-plan/download/excel")]
        public async Task<IActionResult> DownloadSeatPlanExcel(long examId)
        {
            var file = await _mediator.Send(new ExamSeatPlanDownloadExcelQuery(examId));
            return File(file.Content, file.ContentType, file.FileName);
        }

        /// <summary>Downloadable admit card PDF for a single enrollment.</summary>
        [HttpGet("{examId}/enrollments/{enrollmentId}/admit-card/download")]
        public async Task<IActionResult> DownloadAdmitCard(long examId, long enrollmentId)
        {
            var file = await _mediator.Send(new ExamAdmitCardDownloadPdfQuery(enrollmentId));
            return File(file.Content, file.ContentType, file.FileName);
        }
    }
}
