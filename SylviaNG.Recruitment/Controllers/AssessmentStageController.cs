using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.AssessmentStages.Queries.AssessmentStageGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.AssessmentStages.Commands.AssessmentStageCreate;
using SylviaNG.Recruitment.Application.Features.AssessmentStages.Commands.AssessmentStageDelete;
using SylviaNG.Recruitment.Application.Features.AssessmentStages.Commands.AssessmentStageUpdate;
using SylviaNG.Recruitment.Application.Features.AssessmentStages.Models;
using SylviaNG.Recruitment.Application.Features.AssessmentStages.Queries.AssessmentStageGetAll;
using SylviaNG.Recruitment.Application.Features.AssessmentStages.Queries.AssessmentStageGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/assessment-stage")]
    public class AssessmentStageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AssessmentStageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<AssessmentStageResponse>>> GetAll()
        {
            var result = await _mediator.Send(new AssessmentStageGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<AssessmentStageResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new AssessmentStageGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{assessmentStageId}")]
        public async Task<ActionResult<AssessmentStageResponse>> GetById(long assessmentStageId)
        {
            var result = await _mediator.Send(new AssessmentStageGetByIdQuery(assessmentStageId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] AssessmentStageCreateRequest request)
        {
            var id = await _mediator.Send(new AssessmentStageCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{assessmentStageId}")]
        public async Task<ActionResult> Update(long assessmentStageId, [FromBody] AssessmentStageUpdateRequest request)
        {
            await _mediator.Send(new AssessmentStageUpdateCommand(assessmentStageId, request));
            return Ok();
        }

        [HttpDelete("{assessmentStageId}")]
        public async Task<ActionResult> Delete(long assessmentStageId)
        {
            await _mediator.Send(new AssessmentStageDeleteCommand(assessmentStageId));
            return Ok();
        }
    }
}
