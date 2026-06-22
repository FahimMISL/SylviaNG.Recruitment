using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Queries.ExamHallGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallCreate;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallDelete;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallUpdate;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Queries.ExamHallGetAll;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Queries.ExamHallGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/exam-hall")]
    public class ExamHallController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExamHallController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ExamHallResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ExamHallGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ExamHallResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ExamHallGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{examHallId}")]
        public async Task<ActionResult<ExamHallResponse>> GetById(long examHallId)
        {
            var result = await _mediator.Send(new ExamHallGetByIdQuery(examHallId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ExamHallCreateRequest request)
        {
            var id = await _mediator.Send(new ExamHallCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{examHallId}")]
        public async Task<ActionResult> Update(long examHallId, [FromBody] ExamHallUpdateRequest request)
        {
            await _mediator.Send(new ExamHallUpdateCommand(examHallId, request));
            return Ok();
        }

        [HttpDelete("{examHallId}")]
        public async Task<ActionResult> Delete(long examHallId)
        {
            await _mediator.Send(new ExamHallDeleteCommand(examHallId));
            return Ok();
        }
    }
}
