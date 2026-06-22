using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Queries.RequisitionAttachmentGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Commands.RequisitionAttachmentCreate;
using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Commands.RequisitionAttachmentDelete;
using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Commands.RequisitionAttachmentUpdate;
using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Models;
using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Queries.RequisitionAttachmentGetAll;
using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Queries.RequisitionAttachmentGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/requisition-attachment")]
    public class RequisitionAttachmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RequisitionAttachmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<RequisitionAttachmentResponse>>> GetAll()
        {
            var result = await _mediator.Send(new RequisitionAttachmentGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<RequisitionAttachmentResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new RequisitionAttachmentGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{requisitionAttachmentId}")]
        public async Task<ActionResult<RequisitionAttachmentResponse>> GetById(long requisitionAttachmentId)
        {
            var result = await _mediator.Send(new RequisitionAttachmentGetByIdQuery(requisitionAttachmentId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] RequisitionAttachmentCreateRequest request)
        {
            var id = await _mediator.Send(new RequisitionAttachmentCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{requisitionAttachmentId}")]
        public async Task<ActionResult> Update(long requisitionAttachmentId, [FromBody] RequisitionAttachmentUpdateRequest request)
        {
            await _mediator.Send(new RequisitionAttachmentUpdateCommand(requisitionAttachmentId, request));
            return Ok();
        }

        [HttpDelete("{requisitionAttachmentId}")]
        public async Task<ActionResult> Delete(long requisitionAttachmentId)
        {
            await _mediator.Send(new RequisitionAttachmentDeleteCommand(requisitionAttachmentId));
            return Ok();
        }
    }
}
