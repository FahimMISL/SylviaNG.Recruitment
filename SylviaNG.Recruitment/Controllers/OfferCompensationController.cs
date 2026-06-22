using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.OfferCompensations.Queries.OfferCompensationGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.OfferCompensations.Commands.OfferCompensationCreate;
using SylviaNG.Recruitment.Application.Features.OfferCompensations.Commands.OfferCompensationDelete;
using SylviaNG.Recruitment.Application.Features.OfferCompensations.Commands.OfferCompensationUpdate;
using SylviaNG.Recruitment.Application.Features.OfferCompensations.Models;
using SylviaNG.Recruitment.Application.Features.OfferCompensations.Queries.OfferCompensationGetAll;
using SylviaNG.Recruitment.Application.Features.OfferCompensations.Queries.OfferCompensationGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/offer-compensation")]
    public class OfferCompensationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OfferCompensationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<OfferCompensationResponse>>> GetAll()
        {
            var result = await _mediator.Send(new OfferCompensationGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<OfferCompensationResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new OfferCompensationGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{offerCompensationId}")]
        public async Task<ActionResult<OfferCompensationResponse>> GetById(long offerCompensationId)
        {
            var result = await _mediator.Send(new OfferCompensationGetByIdQuery(offerCompensationId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] OfferCompensationCreateRequest request)
        {
            var id = await _mediator.Send(new OfferCompensationCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{offerCompensationId}")]
        public async Task<ActionResult> Update(long offerCompensationId, [FromBody] OfferCompensationUpdateRequest request)
        {
            await _mediator.Send(new OfferCompensationUpdateCommand(offerCompensationId, request));
            return Ok();
        }

        [HttpDelete("{offerCompensationId}")]
        public async Task<ActionResult> Delete(long offerCompensationId)
        {
            await _mediator.Send(new OfferCompensationDeleteCommand(offerCompensationId));
            return Ok();
        }
    }
}
