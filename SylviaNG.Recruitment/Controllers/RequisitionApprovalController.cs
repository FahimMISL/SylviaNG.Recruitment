using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Queries.RequisitionApprovalGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Commands.RequisitionApprovalCreate;
using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Commands.RequisitionApprovalDelete;
using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Commands.RequisitionApprovalUpdate;
using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Models;
using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Queries.RequisitionApprovalGetAll;
using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Queries.RequisitionApprovalGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/requisition-approval")]
    public class RequisitionApprovalController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RequisitionApprovalController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<RequisitionApprovalResponse>>> GetAll()
        {
            var result = await _mediator.Send(new RequisitionApprovalGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<RequisitionApprovalResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new RequisitionApprovalGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{requisitionApprovalId}")]
        public async Task<ActionResult<RequisitionApprovalResponse>> GetById(long requisitionApprovalId)
        {
            var result = await _mediator.Send(new RequisitionApprovalGetByIdQuery(requisitionApprovalId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] RequisitionApprovalCreateRequest request)
        {
            var id = await _mediator.Send(new RequisitionApprovalCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{requisitionApprovalId}")]
        public async Task<ActionResult> Update(long requisitionApprovalId, [FromBody] RequisitionApprovalUpdateRequest request)
        {
            await _mediator.Send(new RequisitionApprovalUpdateCommand(requisitionApprovalId, request));
            return Ok();
        }

        [HttpDelete("{requisitionApprovalId}")]
        public async Task<ActionResult> Delete(long requisitionApprovalId)
        {
            await _mediator.Send(new RequisitionApprovalDeleteCommand(requisitionApprovalId));
            return Ok();
        }
    }
}
