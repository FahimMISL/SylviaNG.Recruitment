using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.SpecialCategories.Commands.SpecialCategoryCreate;
using SylviaNG.Recruitment.Application.Features.SpecialCategories.Commands.SpecialCategoryDelete;
using SylviaNG.Recruitment.Application.Features.SpecialCategories.Commands.SpecialCategoryUpdate;
using SylviaNG.Recruitment.Application.Features.SpecialCategories.Models;
using SylviaNG.Recruitment.Application.Features.SpecialCategories.Queries.SpecialCategoryGetAll;

namespace SylviaNG.Recruitment.Controllers
{
    // GetAll has no [Authorize] - global AuthorizeFilter already requires login, and the apply
    // form's optional Special Category dropdown needs this lookup for any authenticated
    // candidate. Writes are Admin-only (System Administration / master data management).
    [ApiController]
    [Route("recruitment/special-category")]
    public class SpecialCategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SpecialCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<SpecialCategoryResponse>>> GetAll()
        {
            var result = await _mediator.Send(new SpecialCategoryGetAllQuery());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<long>> Create([FromBody] SpecialCategoryCreateRequest request)
        {
            var id = await _mediator.Send(new SpecialCategoryCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{specialCategoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(long specialCategoryId, [FromBody] SpecialCategoryUpdateRequest request)
        {
            await _mediator.Send(new SpecialCategoryUpdateCommand(specialCategoryId, request));
            return Ok();
        }

        [HttpDelete("{specialCategoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(long specialCategoryId)
        {
            await _mediator.Send(new SpecialCategoryDeleteCommand(specialCategoryId));
            return Ok();
        }
    }
}
