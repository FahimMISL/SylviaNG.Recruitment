using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Companies.Commands.CompanyCreate;
using SylviaNG.Recruitment.Application.Features.Companies.Commands.CompanySetActive;
using SylviaNG.Recruitment.Application.Features.Companies.Commands.CompanyUpdate;
using SylviaNG.Recruitment.Application.Features.Companies.Models;
using SylviaNG.Recruitment.Application.Features.Companies.Queries.CompanyGetAll;
using SylviaNG.Recruitment.Application.Features.Companies.Queries.CompanyGetById;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>
    /// Tenant Company CRUD - SuperAdmin-only (a Company Admin/HR manages their own company's
    /// recruitment data, never the company record itself or any other tenant's).
    /// </summary>
    [ApiController]
    [Route("recruitment/companies")]
    [Authorize(Roles = "SuperAdmin")]
    public class CompanyController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICompanyService _companyService;

        public CompanyController(IMediator mediator, ICompanyService companyService)
        {
            _mediator = mediator;
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CompanyResponse>>> GetAll()
        {
            var result = await _mediator.Send(new CompanyGetAllQuery());
            return Ok(result);
        }

        [HttpGet("{companyId}")]
        public async Task<ActionResult<CompanyResponse>> GetById(long companyId)
        {
            var result = await _mediator.Send(new CompanyGetByIdQuery(companyId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] CompanyCreateRequest request)
        {
            var id = await _mediator.Send(new CompanyCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{companyId}")]
        public async Task<ActionResult> Update(long companyId, [FromBody] CompanyUpdateRequest request)
        {
            await _mediator.Send(new CompanyUpdateCommand(companyId, request));
            return Ok();
        }

        [HttpPatch("{companyId}/active")]
        public async Task<ActionResult> SetActive(long companyId, [FromQuery] bool isActive)
        {
            await _mediator.Send(new CompanySetActiveCommand(companyId, isActive));
            return Ok();
        }

        [HttpPost("{companyId}/logo")]
        public async Task<ActionResult<string>> UploadLogo(long companyId, [FromForm] IFormFile file)
        {
            var path = await _companyService.UploadLogoAsync(companyId, file);
            return Ok(path);
        }
    }
}
