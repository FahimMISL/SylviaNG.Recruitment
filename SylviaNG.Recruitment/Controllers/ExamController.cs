using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.Exams.Queries.ExamGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Exams.Commands.ExamCreate;
using SylviaNG.Recruitment.Application.Features.Exams.Commands.ExamDelete;
using SylviaNG.Recruitment.Application.Features.Exams.Commands.ExamUpdate;
using SylviaNG.Recruitment.Application.Features.Exams.Models;
using SylviaNG.Recruitment.Application.Features.Exams.Queries.ExamGetAll;
using SylviaNG.Recruitment.Application.Features.Exams.Queries.ExamGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/exam")]
    public class ExamController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExamController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ExamResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ExamGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ExamResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ExamGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{examId}")]
        public async Task<ActionResult<ExamResponse>> GetById(long examId)
        {
            var result = await _mediator.Send(new ExamGetByIdQuery(examId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ExamCreateRequest request)
        {
            var id = await _mediator.Send(new ExamCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{examId}")]
        public async Task<ActionResult> Update(long examId, [FromBody] ExamUpdateRequest request)
        {
            await _mediator.Send(new ExamUpdateCommand(examId, request));
            return Ok();
        }

        [HttpDelete("{examId}")]
        public async Task<ActionResult> Delete(long examId)
        {
            await _mediator.Send(new ExamDeleteCommand(examId));
            return Ok();
        }
    }
}
