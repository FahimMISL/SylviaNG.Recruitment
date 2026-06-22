using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Queries.ShortlistFilterCriteriaGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Commands.ShortlistFilterCriteriaCreate;
using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Commands.ShortlistFilterCriteriaDelete;
using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Commands.ShortlistFilterCriteriaUpdate;
using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Models;
using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Queries.ShortlistFilterCriteriaGetAll;
using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Queries.ShortlistFilterCriteriaGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/shortlist-filter-criteria")]
    public class ShortlistFilterCriteriaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShortlistFilterCriteriaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ShortlistFilterCriteriaResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ShortlistFilterCriteriaGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ShortlistFilterCriteriaResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ShortlistFilterCriteriaGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{shortlistFilterCriteriaId}")]
        public async Task<ActionResult<ShortlistFilterCriteriaResponse>> GetById(long shortlistFilterCriteriaId)
        {
            var result = await _mediator.Send(new ShortlistFilterCriteriaGetByIdQuery(shortlistFilterCriteriaId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ShortlistFilterCriteriaCreateRequest request)
        {
            var id = await _mediator.Send(new ShortlistFilterCriteriaCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{shortlistFilterCriteriaId}")]
        public async Task<ActionResult> Update(long shortlistFilterCriteriaId, [FromBody] ShortlistFilterCriteriaUpdateRequest request)
        {
            await _mediator.Send(new ShortlistFilterCriteriaUpdateCommand(shortlistFilterCriteriaId, request));
            return Ok();
        }

        [HttpDelete("{shortlistFilterCriteriaId}")]
        public async Task<ActionResult> Delete(long shortlistFilterCriteriaId)
        {
            await _mediator.Send(new ShortlistFilterCriteriaDeleteCommand(shortlistFilterCriteriaId));
            return Ok();
        }
    }
}
