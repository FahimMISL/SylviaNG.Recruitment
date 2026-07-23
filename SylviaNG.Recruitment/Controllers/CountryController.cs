using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Countries.Commands.CountryCreate;
using SylviaNG.Recruitment.Application.Features.Countries.Commands.CountryDelete;
using SylviaNG.Recruitment.Application.Features.Countries.Commands.CountryUpdate;
using SylviaNG.Recruitment.Application.Features.Countries.Models;
using SylviaNG.Recruitment.Application.Features.Countries.Queries.CountryGetAll;

namespace SylviaNG.Recruitment.Controllers
{
    // GetAll has no [Authorize] - global AuthorizeFilter already requires login, and any
    // authenticated role (candidate filling the contact form's country-code dropdown) needs this
    // lookup. Writes are Admin-only (System Administration / master data management).
    [ApiController]
    [Route("recruitment/country")]
    public class CountryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CountryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CountryResponse>>> GetAll()
        {
            var result = await _mediator.Send(new CountryGetAllQuery());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<long>> Create([FromBody] CountryCreateRequest request)
        {
            var id = await _mediator.Send(new CountryCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{countryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(long countryId, [FromBody] CountryUpdateRequest request)
        {
            await _mediator.Send(new CountryUpdateCommand(countryId, request));
            return Ok();
        }

        [HttpDelete("{countryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(long countryId)
        {
            await _mediator.Send(new CountryDeleteCommand(countryId));
            return Ok();
        }
    }
}
