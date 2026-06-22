using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.Requisitions.Queries.RequisitionGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Requisitions.Commands.RequisitionCreate;
using SylviaNG.Recruitment.Application.Features.Requisitions.Commands.RequisitionDelete;
using SylviaNG.Recruitment.Application.Features.Requisitions.Commands.RequisitionUpdate;
using SylviaNG.Recruitment.Application.Features.Requisitions.Models;
using SylviaNG.Recruitment.Application.Features.Requisitions.Queries.RequisitionGetAll;
using SylviaNG.Recruitment.Application.Features.Requisitions.Queries.RequisitionGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/requisition")]
    public class RequisitionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RequisitionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<RequisitionResponse>>> GetAll()
        {
            var result = await _mediator.Send(new RequisitionGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<RequisitionResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new RequisitionGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{requisitionId}")]
        public async Task<ActionResult<RequisitionResponse>> GetById(long requisitionId)
        {
            var result = await _mediator.Send(new RequisitionGetByIdQuery(requisitionId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] RequisitionCreateRequest request)
        {
            var id = await _mediator.Send(new RequisitionCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{requisitionId}")]
        public async Task<ActionResult> Update(long requisitionId, [FromBody] RequisitionUpdateRequest request)
        {
            await _mediator.Send(new RequisitionUpdateCommand(requisitionId, request));
            return Ok();
        }

        [HttpDelete("{requisitionId}")]
        public async Task<ActionResult> Delete(long requisitionId)
        {
            await _mediator.Send(new RequisitionDeleteCommand(requisitionId));
            return Ok();
        }
    }
}
