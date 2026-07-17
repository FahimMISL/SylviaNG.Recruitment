using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowCreate;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowDelete;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowSetActive;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowUpdate;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Queries.AssessmentWorkflowGetActiveLookup;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Queries.AssessmentWorkflowGetAll;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Queries.AssessmentWorkflowGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [ApiController]
    [Route("recruitment/assessment-workflow")]
    [Authorize(Roles = "Admin,HR")]
    public class AssessmentWorkflowController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AssessmentWorkflowController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all assessment workflows with their stages.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<AssessmentWorkflowResponse>>> GetAll()
        {
            var result = await _mediator.Send(new AssessmentWorkflowGetAllQuery());
            return Ok(result);
        }

        /// <summary>
        /// Active workflows only (Id + Name), for the "assign workflow to job posting" dropdown.
        /// </summary>
        [HttpGet("active-lookup")]
        public async Task<ActionResult<List<AssessmentWorkflowLookupResponse>>> GetActiveLookup()
        {
            var result = await _mediator.Send(new AssessmentWorkflowGetActiveLookupQuery());
            return Ok(result);
        }

        /// <summary>
        /// Get an assessment workflow by ID, including its ordered stages.
        /// </summary>
        [HttpGet("{assessmentWorkflowId}")]
        public async Task<ActionResult<AssessmentWorkflowResponse>> GetById(long assessmentWorkflowId)
        {
            var result = await _mediator.Send(new AssessmentWorkflowGetByIdQuery(assessmentWorkflowId));
            return Ok(result);
        }

        /// <summary>
        /// Create a new assessment workflow with its stages.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] AssessmentWorkflowCreateRequest request)
        {
            var id = await _mediator.Send(new AssessmentWorkflowCreateCommand(request));
            return Ok(id);
        }

        /// <summary>
        /// Update an assessment workflow. The stage list is a full replacement (add/edit/remove/reorder in one save).
        /// </summary>
        [HttpPut("{assessmentWorkflowId}")]
        public async Task<ActionResult> Update(long assessmentWorkflowId, [FromBody] AssessmentWorkflowUpdateRequest request)
        {
            await _mediator.Send(new AssessmentWorkflowUpdateCommand(assessmentWorkflowId, request));
            return Ok();
        }

        /// <summary>
        /// Delete an assessment workflow. Fails with 409 if any job posting is still assigned to it.
        /// </summary>
        [HttpDelete("{assessmentWorkflowId}")]
        public async Task<ActionResult> Delete(long assessmentWorkflowId)
        {
            await _mediator.Send(new AssessmentWorkflowDeleteCommand(assessmentWorkflowId));
            return Ok();
        }

        /// <summary>
        /// Activate or deactivate a workflow without touching its stages.
        /// </summary>
        [HttpPatch("{assessmentWorkflowId}/active")]
        public async Task<ActionResult> SetActive(long assessmentWorkflowId, [FromQuery] bool isActive)
        {
            await _mediator.Send(new AssessmentWorkflowSetActiveCommand(assessmentWorkflowId, isActive));
            return Ok();
        }
    }
}
