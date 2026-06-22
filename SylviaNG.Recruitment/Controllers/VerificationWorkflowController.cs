using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Queries.VerificationWorkflowGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Commands.VerificationWorkflowCreate;
using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Commands.VerificationWorkflowDelete;
using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Commands.VerificationWorkflowUpdate;
using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Models;
using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Queries.VerificationWorkflowGetAll;
using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Queries.VerificationWorkflowGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/verification-workflow")]
    public class VerificationWorkflowController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VerificationWorkflowController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<VerificationWorkflowResponse>>> GetAll()
        {
            var result = await _mediator.Send(new VerificationWorkflowGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<VerificationWorkflowResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new VerificationWorkflowGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{verificationWorkflowId}")]
        public async Task<ActionResult<VerificationWorkflowResponse>> GetById(long verificationWorkflowId)
        {
            var result = await _mediator.Send(new VerificationWorkflowGetByIdQuery(verificationWorkflowId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] VerificationWorkflowCreateRequest request)
        {
            var id = await _mediator.Send(new VerificationWorkflowCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{verificationWorkflowId}")]
        public async Task<ActionResult> Update(long verificationWorkflowId, [FromBody] VerificationWorkflowUpdateRequest request)
        {
            await _mediator.Send(new VerificationWorkflowUpdateCommand(verificationWorkflowId, request));
            return Ok();
        }

        [HttpDelete("{verificationWorkflowId}")]
        public async Task<ActionResult> Delete(long verificationWorkflowId)
        {
            await _mediator.Send(new VerificationWorkflowDeleteCommand(verificationWorkflowId));
            return Ok();
        }
    }
}
