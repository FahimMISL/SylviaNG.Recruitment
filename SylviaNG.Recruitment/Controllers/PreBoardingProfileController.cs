using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Queries.PreBoardingProfileGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Commands.PreBoardingProfileCreate;
using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Commands.PreBoardingProfileDelete;
using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Commands.PreBoardingProfileUpdate;
using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Models;
using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Queries.PreBoardingProfileGetAll;
using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Queries.PreBoardingProfileGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/pre-boarding-profile")]
    public class PreBoardingProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PreBoardingProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<PreBoardingProfileResponse>>> GetAll()
        {
            var result = await _mediator.Send(new PreBoardingProfileGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<PreBoardingProfileResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new PreBoardingProfileGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{preBoardingProfileId}")]
        public async Task<ActionResult<PreBoardingProfileResponse>> GetById(long preBoardingProfileId)
        {
            var result = await _mediator.Send(new PreBoardingProfileGetByIdQuery(preBoardingProfileId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] PreBoardingProfileCreateRequest request)
        {
            var id = await _mediator.Send(new PreBoardingProfileCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{preBoardingProfileId}")]
        public async Task<ActionResult> Update(long preBoardingProfileId, [FromBody] PreBoardingProfileUpdateRequest request)
        {
            await _mediator.Send(new PreBoardingProfileUpdateCommand(preBoardingProfileId, request));
            return Ok();
        }

        [HttpDelete("{preBoardingProfileId}")]
        public async Task<ActionResult> Delete(long preBoardingProfileId)
        {
            await _mediator.Send(new PreBoardingProfileDeleteCommand(preBoardingProfileId));
            return Ok();
        }
    }
}
