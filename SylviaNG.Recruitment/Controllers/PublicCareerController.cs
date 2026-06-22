using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Controllers;

[AllowAnonymous]
[ApiController]
[Route("recruitment/public-career")]
public class PublicCareerController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public PublicCareerController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("jobs")]
    public async Task<IActionResult> GetOpenJobs([FromQuery] int page = 1, [FromQuery] int pageSize = 12, [FromQuery] string? search = null)
    {
        if (pageSize > 50) pageSize = 50;
        if (page < 1) page = 1;

        var query = _unitOfWork.Context.Set<JobPosting>()
            .Where(j => j.IsActive && j.Status == JobStatusEnum.Open);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(j => j.Title.Contains(search) || (j.Description != null && j.Description.Contains(search)));

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(j => j.PostingDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => new
            {
                j.JobPostingId,
                j.Title,
                j.Description,
                j.Location,
                j.EmploymentType,
                j.MinSalary,
                j.MaxSalary,
                j.PostingDate,
                j.ClosingDate,
                j.NumberOfPositions,
                j.MinAge,
                j.MaxAge,
                j.MinExperienceYears,
                j.MinEducationLevel,
                j.RequiredDistrict
            })
            .ToListAsync();

        return Ok(new { items, totalCount = total, page, pageSize });
    }

    [HttpGet("jobs/{id}")]
    public async Task<IActionResult> GetJobDetail(long id)
    {
        var job = await _unitOfWork.Context.Set<JobPosting>()
            .Where(j => j.JobPostingId == id && j.IsActive && j.Status == JobStatusEnum.Open)
            .Select(j => new
            {
                j.JobPostingId,
                j.Title,
                j.Description,
                j.Requirements,
                j.Location,
                j.EmploymentType,
                j.MinSalary,
                j.MaxSalary,
                j.PostingDate,
                j.ClosingDate,
                j.NumberOfPositions,
                j.MinAge,
                j.MaxAge,
                j.MinExperienceYears,
                j.MinEducationLevel,
                j.RequiredDistrict
            })
            .FirstOrDefaultAsync();

        if (job == null) return NotFound();
        return Ok(job);
    }

    [HttpGet("content")]
    public async Task<IActionResult> GetPublishedContent()
    {
        var content = await _unitOfWork.Context.Set<CareerContent>()
            .Where(c => c.IsActive && c.IsPublished)
            .OrderBy(c => c.SortOrder)
            .Select(c => new
            {
                c.CareerContentId,
                c.ContentType,
                c.Title,
                c.Body,
                c.MediaUrl,
                c.SortOrder
            })
            .ToListAsync();

        return Ok(content);
    }

    [HttpGet("content/{type}")]
    public async Task<IActionResult> GetContentByType(string type)
    {
        if (!Enum.TryParse<CareerContentTypeEnum>(type, true, out var contentType))
            return BadRequest("Invalid content type.");

        var content = await _unitOfWork.Context.Set<CareerContent>()
            .Where(c => c.IsActive && c.IsPublished && c.ContentType == contentType)
            .OrderBy(c => c.SortOrder)
            .Select(c => new
            {
                c.CareerContentId,
                c.ContentType,
                c.Title,
                c.Body,
                c.MediaUrl,
                c.SortOrder
            })
            .ToListAsync();

        return Ok(content);
    }
}
