using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CareerContents.Queries.CareerContentGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.CareerContents.Commands.CareerContentCreate;
using SylviaNG.Recruitment.Application.Features.CareerContents.Commands.CareerContentDelete;
using SylviaNG.Recruitment.Application.Features.CareerContents.Commands.CareerContentUpdate;
using SylviaNG.Recruitment.Application.Features.CareerContents.Models;
using SylviaNG.Recruitment.Application.Features.CareerContents.Queries.CareerContentGetAll;
using SylviaNG.Recruitment.Application.Features.CareerContents.Queries.CareerContentGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/career-content")]
    public class CareerContentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CareerContentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CareerContentResponse>>> GetAll()
        {
            var result = await _mediator.Send(new CareerContentGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<CareerContentResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new CareerContentGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{careerContentId}")]
        public async Task<ActionResult<CareerContentResponse>> GetById(long careerContentId)
        {
            var result = await _mediator.Send(new CareerContentGetByIdQuery(careerContentId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] CareerContentCreateRequest request)
        {
            var id = await _mediator.Send(new CareerContentCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{careerContentId}")]
        public async Task<ActionResult> Update(long careerContentId, [FromBody] CareerContentUpdateRequest request)
        {
            await _mediator.Send(new CareerContentUpdateCommand(careerContentId, request));
            return Ok();
        }

        [HttpDelete("{careerContentId}")]
        public async Task<ActionResult> Delete(long careerContentId)
        {
            await _mediator.Send(new CareerContentDeleteCommand(careerContentId));
            return Ok();
        }
    }
}
