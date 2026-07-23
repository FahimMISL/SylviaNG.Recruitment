using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ApplicationSettings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>
    /// Tenant-wide recruitment settings (US-007 AC4: minimum profile completeness required to
    /// submit an application). No class-level [Authorize] beyond the global AuthorizeFilter - GET
    /// is readable by any authenticated role so the candidate UI can show "X% required", write is
    /// Admin-only.
    /// </summary>
    [ApiController]
    [Route("recruitment/application-settings")]
    public class ApplicationSettingController : ControllerBase
    {
        private readonly IApplicationSettingService _applicationSettingService;

        public ApplicationSettingController(IApplicationSettingService applicationSettingService)
        {
            _applicationSettingService = applicationSettingService;
        }

        [HttpGet]
        public async Task<ActionResult<ApplicationSettingResponse>> Get()
        {
            var result = await _applicationSettingService.GetAsync();
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update([FromBody] ApplicationSettingUpdateRequest request)
        {
            await _applicationSettingService.UpdateAsync(request);
            return Ok();
        }
    }
}
