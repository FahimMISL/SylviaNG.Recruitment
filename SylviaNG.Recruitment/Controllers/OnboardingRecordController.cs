using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Queries.OnboardingRecordGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Commands.OnboardingRecordCreate;
using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Commands.OnboardingRecordDelete;
using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Commands.OnboardingRecordUpdate;
using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Models;
using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Queries.OnboardingRecordGetAll;
using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Queries.OnboardingRecordGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/onboarding-record")]
    public class OnboardingRecordController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OnboardingRecordController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<OnboardingRecordResponse>>> GetAll()
        {
            var result = await _mediator.Send(new OnboardingRecordGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<OnboardingRecordResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new OnboardingRecordGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{onboardingRecordId}")]
        public async Task<ActionResult<OnboardingRecordResponse>> GetById(long onboardingRecordId)
        {
            var result = await _mediator.Send(new OnboardingRecordGetByIdQuery(onboardingRecordId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] OnboardingRecordCreateRequest request)
        {
            var id = await _mediator.Send(new OnboardingRecordCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{onboardingRecordId}")]
        public async Task<ActionResult> Update(long onboardingRecordId, [FromBody] OnboardingRecordUpdateRequest request)
        {
            await _mediator.Send(new OnboardingRecordUpdateCommand(onboardingRecordId, request));
            return Ok();
        }

        [HttpDelete("{onboardingRecordId}")]
        public async Task<ActionResult> Delete(long onboardingRecordId)
        {
            await _mediator.Send(new OnboardingRecordDeleteCommand(onboardingRecordId));
            return Ok();
        }
    }
}
