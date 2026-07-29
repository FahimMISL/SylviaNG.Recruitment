using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Commands.FitmentDataUpsert;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Queries.FitmentDataGetByJobApplication;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-12 US-097: manual-entry grade/designation/salary structure per JobApplication.
    [ApiController]
    [Route("recruitment/fitment-data")]
    [Authorize(Roles = "Admin,HR")]
    public class FitmentDataController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FitmentDataController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("by-job-application/{jobApplicationId}")]
        public async Task<ActionResult<FitmentDataResponse?>> GetByJobApplication(long jobApplicationId)
        {
            return Ok(await _mediator.Send(new FitmentDataGetByJobApplicationQuery(jobApplicationId)));
        }

        [HttpPut]
        public async Task<ActionResult<FitmentDataResponse>> Upsert([FromBody] FitmentDataUpsertRequest request)
        {
            return Ok(await _mediator.Send(new FitmentDataUpsertCommand(request)));
        }
    }
}
