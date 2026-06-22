using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.AdmitCards.Queries.AdmitCardGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.AdmitCards.Commands.AdmitCardCreate;
using SylviaNG.Recruitment.Application.Features.AdmitCards.Commands.AdmitCardDelete;
using SylviaNG.Recruitment.Application.Features.AdmitCards.Commands.AdmitCardUpdate;
using SylviaNG.Recruitment.Application.Features.AdmitCards.Models;
using SylviaNG.Recruitment.Application.Features.AdmitCards.Queries.AdmitCardGetAll;
using SylviaNG.Recruitment.Application.Features.AdmitCards.Queries.AdmitCardGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/admit-card")]
    public class AdmitCardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdmitCardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<AdmitCardResponse>>> GetAll()
        {
            var result = await _mediator.Send(new AdmitCardGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<AdmitCardResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new AdmitCardGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{admitCardId}")]
        public async Task<ActionResult<AdmitCardResponse>> GetById(long admitCardId)
        {
            var result = await _mediator.Send(new AdmitCardGetByIdQuery(admitCardId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] AdmitCardCreateRequest request)
        {
            var id = await _mediator.Send(new AdmitCardCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{admitCardId}")]
        public async Task<ActionResult> Update(long admitCardId, [FromBody] AdmitCardUpdateRequest request)
        {
            await _mediator.Send(new AdmitCardUpdateCommand(admitCardId, request));
            return Ok();
        }

        [HttpDelete("{admitCardId}")]
        public async Task<ActionResult> Delete(long admitCardId)
        {
            await _mediator.Send(new AdmitCardDeleteCommand(admitCardId));
            return Ok();
        }
    }
}
