using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ExamCandidates.Queries.ExamCandidateGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ExamCandidates.Commands.ExamCandidateCreate;
using SylviaNG.Recruitment.Application.Features.ExamCandidates.Commands.ExamCandidateDelete;
using SylviaNG.Recruitment.Application.Features.ExamCandidates.Commands.ExamCandidateUpdate;
using SylviaNG.Recruitment.Application.Features.ExamCandidates.Models;
using SylviaNG.Recruitment.Application.Features.ExamCandidates.Queries.ExamCandidateGetAll;
using SylviaNG.Recruitment.Application.Features.ExamCandidates.Queries.ExamCandidateGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/exam-candidate")]
    public class ExamCandidateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExamCandidateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ExamCandidateResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ExamCandidateGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ExamCandidateResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ExamCandidateGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{examCandidateId}")]
        public async Task<ActionResult<ExamCandidateResponse>> GetById(long examCandidateId)
        {
            var result = await _mediator.Send(new ExamCandidateGetByIdQuery(examCandidateId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ExamCandidateCreateRequest request)
        {
            var id = await _mediator.Send(new ExamCandidateCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{examCandidateId}")]
        public async Task<ActionResult> Update(long examCandidateId, [FromBody] ExamCandidateUpdateRequest request)
        {
            await _mediator.Send(new ExamCandidateUpdateCommand(examCandidateId, request));
            return Ok();
        }

        [HttpDelete("{examCandidateId}")]
        public async Task<ActionResult> Delete(long examCandidateId)
        {
            await _mediator.Send(new ExamCandidateDeleteCommand(examCandidateId));
            return Ok();
        }
    }
}
