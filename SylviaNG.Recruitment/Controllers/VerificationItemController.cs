using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.VerificationItems.Queries.VerificationItemGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.VerificationItems.Commands.VerificationItemCreate;
using SylviaNG.Recruitment.Application.Features.VerificationItems.Commands.VerificationItemDelete;
using SylviaNG.Recruitment.Application.Features.VerificationItems.Commands.VerificationItemUpdate;
using SylviaNG.Recruitment.Application.Features.VerificationItems.Models;
using SylviaNG.Recruitment.Application.Features.VerificationItems.Queries.VerificationItemGetAll;
using SylviaNG.Recruitment.Application.Features.VerificationItems.Queries.VerificationItemGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/verification-item")]
    public class VerificationItemController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VerificationItemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<VerificationItemResponse>>> GetAll()
        {
            var result = await _mediator.Send(new VerificationItemGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<VerificationItemResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new VerificationItemGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{verificationItemId}")]
        public async Task<ActionResult<VerificationItemResponse>> GetById(long verificationItemId)
        {
            var result = await _mediator.Send(new VerificationItemGetByIdQuery(verificationItemId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] VerificationItemCreateRequest request)
        {
            var id = await _mediator.Send(new VerificationItemCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{verificationItemId}")]
        public async Task<ActionResult> Update(long verificationItemId, [FromBody] VerificationItemUpdateRequest request)
        {
            await _mediator.Send(new VerificationItemUpdateCommand(verificationItemId, request));
            return Ok();
        }

        [HttpDelete("{verificationItemId}")]
        public async Task<ActionResult> Delete(long verificationItemId)
        {
            await _mediator.Send(new VerificationItemDeleteCommand(verificationItemId));
            return Ok();
        }
    }
}
