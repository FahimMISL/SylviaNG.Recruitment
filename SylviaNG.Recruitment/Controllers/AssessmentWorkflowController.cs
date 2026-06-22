using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Queries.AssessmentWorkflowGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowCreate;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowDelete;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowUpdate;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Queries.AssessmentWorkflowGetAll;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Queries.AssessmentWorkflowGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/assessment-workflow")]
    public class AssessmentWorkflowController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AssessmentWorkflowController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<AssessmentWorkflowResponse>>> GetAll()
        {
            var result = await _mediator.Send(new AssessmentWorkflowGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<AssessmentWorkflowResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new AssessmentWorkflowGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{assessmentWorkflowId}")]
        public async Task<ActionResult<AssessmentWorkflowResponse>> GetById(long assessmentWorkflowId)
        {
            var result = await _mediator.Send(new AssessmentWorkflowGetByIdQuery(assessmentWorkflowId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] AssessmentWorkflowCreateRequest request)
        {
            var id = await _mediator.Send(new AssessmentWorkflowCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{assessmentWorkflowId}")]
        public async Task<ActionResult> Update(long assessmentWorkflowId, [FromBody] AssessmentWorkflowUpdateRequest request)
        {
            await _mediator.Send(new AssessmentWorkflowUpdateCommand(assessmentWorkflowId, request));
            return Ok();
        }

        [HttpDelete("{assessmentWorkflowId}")]
        public async Task<ActionResult> Delete(long assessmentWorkflowId)
        {
            await _mediator.Send(new AssessmentWorkflowDeleteCommand(assessmentWorkflowId));
            return Ok();
        }
    }
}
