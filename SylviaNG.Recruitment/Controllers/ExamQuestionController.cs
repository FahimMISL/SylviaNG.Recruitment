using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Commands.ExamQuestionBulkImport;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Commands.ExamQuestionCreate;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Commands.ExamQuestionSetActiveStatus;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Commands.ExamQuestionUpdate;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Models;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Queries.ExamQuestionGetAllPaged;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Queries.ExamQuestionGetById;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Queries.ExamQuestionImportTemplateDownload;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Controllers
{
    [ApiController]
    [Route("recruitment/exam-question")]
    [Authorize(Roles = "Admin,HR")]
    public class ExamQuestionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExamQuestionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Searched/filtered/paged exam question list (US-053 AC5).</summary>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ExamQuestionResponse>>> GetPaged(
            [FromQuery] PagedRequest request,
            [FromQuery] long? questionGroupId,
            [FromQuery] QuestionTypeEnum? questionType,
            [FromQuery] DifficultyLevelEnum? difficultyLevel,
            [FromQuery] bool? isActive)
        {
            var result = await _mediator.Send(new ExamQuestionGetAllPagedQuery(request, questionGroupId, questionType, difficultyLevel, isActive));
            return Ok(result);
        }

        /// <summary>Get an exam question by ID, including its ordered options.</summary>
        [HttpGet("{examQuestionId}")]
        public async Task<ActionResult<ExamQuestionResponse>> GetById(long examQuestionId)
        {
            var result = await _mediator.Send(new ExamQuestionGetByIdQuery(examQuestionId));
            return Ok(result);
        }

        /// <summary>Create a new exam question with its options.</summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ExamQuestionCreateRequest request)
        {
            var id = await _mediator.Send(new ExamQuestionCreateCommand(request));
            return Ok(id);
        }

        /// <summary>Update an exam question. The options list is a full replacement (add/edit/remove in one save).</summary>
        [HttpPut("{examQuestionId}")]
        public async Task<ActionResult> Update(long examQuestionId, [FromBody] ExamQuestionUpdateRequest request)
        {
            await _mediator.Send(new ExamQuestionUpdateCommand(examQuestionId, request));
            return Ok();
        }

        /// <summary>Activate or deactivate an exam question (US-053 AC5).</summary>
        [HttpPatch("{examQuestionId}/active-status")]
        public async Task<ActionResult> SetActiveStatus(long examQuestionId, [FromBody] ExamQuestionSetActiveStatusRequest request)
        {
            await _mediator.Send(new ExamQuestionSetActiveStatusCommand(examQuestionId, request.IsActive));
            return Ok();
        }

        /// <summary>Downloadable XLSX import template (US-054 AC1).</summary>
        [HttpGet("import-template")]
        public async Task<IActionResult> DownloadImportTemplate()
        {
            var template = await _mediator.Send(new ExamQuestionImportTemplateDownloadQuery());
            return File(template.Content, template.ContentType, template.FileName);
        }

        /// <summary>Bulk import exam questions from an XLSX/CSV file into one question group (US-054).</summary>
        [HttpPost("bulk-import")]
        public async Task<ActionResult<ExamQuestionBulkImportResponse>> BulkImport([FromForm] long questionGroupId, [FromForm] IFormFile file)
        {
            var result = await _mediator.Send(new ExamQuestionBulkImportCommand(questionGroupId, file));
            return Ok(result);
        }
    }
}
