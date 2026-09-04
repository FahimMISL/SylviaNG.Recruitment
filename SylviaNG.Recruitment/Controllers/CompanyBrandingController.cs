using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.CompanyBranding.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>
    /// EP-18 F3: admin-facing branding settings (logo, identity/contact, colors, font, border,
    /// watermark, page numbers) - unlike ApplicationSettingController, nothing else in this app
    /// reads this, so every action is Admin-only.
    /// </summary>
    [ApiController]
    [Route("recruitment/company-branding")]
    [Authorize(Roles = "Admin")]
    public class CompanyBrandingController : ControllerBase
    {
        private readonly ICompanyBrandingService _companyBrandingService;

        public CompanyBrandingController(ICompanyBrandingService companyBrandingService)
        {
            _companyBrandingService = companyBrandingService;
        }

        [HttpGet]
        public async Task<ActionResult<CompanyBrandingResponse>> Get()
        {
            var result = await _companyBrandingService.GetAsync();
            return Ok(result);
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] CompanyBrandingUpdateRequest request)
        {
            await _companyBrandingService.UpdateAsync(request);
            return Ok();
        }

        [HttpPost("logo")]
        public async Task<ActionResult<string>> UploadLogo([FromForm] IFormFile file)
        {
            var path = await _companyBrandingService.UploadLogoAsync(file);
            return Ok(path);
        }

        [HttpPost("preview/pdf")]
        public async Task<IActionResult> PreviewPdf([FromBody] CompanyBrandingUpdateRequest request)
        {
            var content = await _companyBrandingService.GeneratePreviewPdfAsync(request);
            return File(content, "application/pdf", "branding-preview.pdf");
        }
    }
}
