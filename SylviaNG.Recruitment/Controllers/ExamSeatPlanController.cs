using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Queries.ExamSeatPlanGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Commands.ExamSeatPlanCreate;
using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Commands.ExamSeatPlanDelete;
using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Commands.ExamSeatPlanUpdate;
using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Models;
using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Queries.ExamSeatPlanGetAll;
using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Queries.ExamSeatPlanGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/exam-seat-plan")]
    public class ExamSeatPlanController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExamSeatPlanController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ExamSeatPlanResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ExamSeatPlanGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ExamSeatPlanResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ExamSeatPlanGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{examSeatPlanId}")]
        public async Task<ActionResult<ExamSeatPlanResponse>> GetById(long examSeatPlanId)
        {
            var result = await _mediator.Send(new ExamSeatPlanGetByIdQuery(examSeatPlanId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ExamSeatPlanCreateRequest request)
        {
            var id = await _mediator.Send(new ExamSeatPlanCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{examSeatPlanId}")]
        public async Task<ActionResult> Update(long examSeatPlanId, [FromBody] ExamSeatPlanUpdateRequest request)
        {
            await _mediator.Send(new ExamSeatPlanUpdateCommand(examSeatPlanId, request));
            return Ok();
        }

        [HttpDelete("{examSeatPlanId}")]
        public async Task<ActionResult> Delete(long examSeatPlanId)
        {
            await _mediator.Send(new ExamSeatPlanDeleteCommand(examSeatPlanId));
            return Ok();
        }
    }
}
