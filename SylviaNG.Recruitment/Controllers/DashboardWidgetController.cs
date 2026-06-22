using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Queries.DashboardWidgetGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Commands.DashboardWidgetCreate;
using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Commands.DashboardWidgetDelete;
using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Commands.DashboardWidgetUpdate;
using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Models;
using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Queries.DashboardWidgetGetAll;
using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Queries.DashboardWidgetGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/dashboard-widget")]
    public class DashboardWidgetController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardWidgetController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<DashboardWidgetResponse>>> GetAll()
        {
            var result = await _mediator.Send(new DashboardWidgetGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<DashboardWidgetResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new DashboardWidgetGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{dashboardWidgetId}")]
        public async Task<ActionResult<DashboardWidgetResponse>> GetById(long dashboardWidgetId)
        {
            var result = await _mediator.Send(new DashboardWidgetGetByIdQuery(dashboardWidgetId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] DashboardWidgetCreateRequest request)
        {
            var id = await _mediator.Send(new DashboardWidgetCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{dashboardWidgetId}")]
        public async Task<ActionResult> Update(long dashboardWidgetId, [FromBody] DashboardWidgetUpdateRequest request)
        {
            await _mediator.Send(new DashboardWidgetUpdateCommand(dashboardWidgetId, request));
            return Ok();
        }

        [HttpDelete("{dashboardWidgetId}")]
        public async Task<ActionResult> Delete(long dashboardWidgetId)
        {
            await _mediator.Send(new DashboardWidgetDeleteCommand(dashboardWidgetId));
            return Ok();
        }
    }
}
