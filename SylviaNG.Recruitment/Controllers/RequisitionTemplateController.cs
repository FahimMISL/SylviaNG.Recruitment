using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Queries.RequisitionTemplateGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Commands.RequisitionTemplateCreate;
using SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Commands.RequisitionTemplateDelete;
using SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Commands.RequisitionTemplateUpdate;
using SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Models;
using SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Queries.RequisitionTemplateGetAll;
using SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Queries.RequisitionTemplateGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/requisition-template")]
    public class RequisitionTemplateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RequisitionTemplateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<RequisitionTemplateResponse>>> GetAll()
        {
            var result = await _mediator.Send(new RequisitionTemplateGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<RequisitionTemplateResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new RequisitionTemplateGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{requisitionTemplateId}")]
        public async Task<ActionResult<RequisitionTemplateResponse>> GetById(long requisitionTemplateId)
        {
            var result = await _mediator.Send(new RequisitionTemplateGetByIdQuery(requisitionTemplateId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] RequisitionTemplateCreateRequest request)
        {
            var id = await _mediator.Send(new RequisitionTemplateCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{requisitionTemplateId}")]
        public async Task<ActionResult> Update(long requisitionTemplateId, [FromBody] RequisitionTemplateUpdateRequest request)
        {
            await _mediator.Send(new RequisitionTemplateUpdateCommand(requisitionTemplateId, request));
            return Ok();
        }

        [HttpDelete("{requisitionTemplateId}")]
        public async Task<ActionResult> Delete(long requisitionTemplateId)
        {
            await _mediator.Send(new RequisitionTemplateDeleteCommand(requisitionTemplateId));
            return Ok();
        }
    }
}
