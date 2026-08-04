using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-15/US-116: mandatory/optional/hidden candidate profile fields, global or per job
    // posting. Writes are Admin-only; the "effective" merged lookup is anonymous because it
    // drives the public/guest career-portal apply form, same reasoning as CareerPortalController.
    [ApiController]
    [Route("recruitment/profile-field-config")]
    [Authorize(Roles = "Admin")]
    public class ProfileFieldConfigController : ControllerBase
    {
        private readonly IProfileFieldConfigService _profileFieldConfigService;

        public ProfileFieldConfigController(IProfileFieldConfigService profileFieldConfigService)
        {
            _profileFieldConfigService = profileFieldConfigService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProfileFieldConfigResponse>>> GetAll()
        {
            var result = await _profileFieldConfigService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>Merged field visibility for rendering an apply/profile form.</summary>
        [HttpGet("effective")]
        [AllowAnonymous]
        public async Task<ActionResult<List<EffectiveProfileFieldResponse>>> GetEffective([FromQuery] long? jobPostingId)
        {
            var result = await _profileFieldConfigService.GetEffectiveConfigAsync(jobPostingId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ProfileFieldConfigRequest request)
        {
            var id = await _profileFieldConfigService.CreateAsync(request);
            return Ok(id);
        }

        [HttpPut("{profileFieldConfigId}")]
        public async Task<ActionResult> Update(long profileFieldConfigId, [FromBody] ProfileFieldConfigRequest request)
        {
            await _profileFieldConfigService.UpdateAsync(profileFieldConfigId, request);
            return Ok();
        }

        [HttpDelete("{profileFieldConfigId}")]
        public async Task<ActionResult> Delete(long profileFieldConfigId)
        {
            await _profileFieldConfigService.DeleteAsync(profileFieldConfigId);
            return Ok();
        }
    }
}
