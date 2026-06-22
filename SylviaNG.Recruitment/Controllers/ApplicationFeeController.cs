using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Controllers;

[Authorize(Roles = "Admin,HR")]
[ApiController]
[Route("recruitment/application-fee")]
public class ApplicationFeeController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ApplicationFeeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var fees = await _unitOfWork.Context.Set<ApplicationFee>()
            .Where(f => f.IsActive)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new
            {
                f.ApplicationFeeId,
                f.JobPostingId,
                JobTitle = f.JobPosting != null ? f.JobPosting.Title : "",
                f.Amount,
                f.Currency,
                f.PaymentMethods,
                f.WaiverRules,
                f.IsActive
            })
            .ToListAsync();

        return Ok(fees);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var fee = await _unitOfWork.Context.Set<ApplicationFee>()
            .Where(f => f.ApplicationFeeId == id)
            .Select(f => new
            {
                f.ApplicationFeeId,
                f.JobPostingId,
                JobTitle = f.JobPosting != null ? f.JobPosting.Title : "",
                f.Amount,
                f.Currency,
                f.PaymentMethods,
                f.WaiverRules,
                f.IsActive
            })
            .FirstOrDefaultAsync();

        if (fee == null) return NotFound();
        return Ok(fee);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ApplicationFeeRequest request)
    {
        if (request.Amount <= 0) return BadRequest("Amount must be greater than zero.");
        if (request.JobPostingId <= 0) return BadRequest("A valid job posting must be selected.");

        var exists = await _unitOfWork.Context.Set<ApplicationFee>()
            .AnyAsync(f => f.JobPostingId == request.JobPostingId && f.IsActive);
        if (exists) return BadRequest("An application fee already exists for this job posting.");

        var fee = new ApplicationFee
        {
            JobPostingId = request.JobPostingId,
            Amount = request.Amount,
            Currency = request.Currency ?? "BDT",
            PaymentMethods = request.PaymentMethods,
            WaiverRules = request.WaiverRules,
            IsActive = true
        };

        await _unitOfWork.Context.Set<ApplicationFee>().AddAsync(fee);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { fee.ApplicationFeeId });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] ApplicationFeeRequest request)
    {
        if (request.Amount <= 0) return BadRequest("Amount must be greater than zero.");

        var fee = await _unitOfWork.Context.Set<ApplicationFee>().FindAsync(id);
        if (fee == null) return NotFound();

        fee.Amount = request.Amount;
        fee.Currency = request.Currency ?? fee.Currency;
        fee.PaymentMethods = request.PaymentMethods;
        fee.WaiverRules = request.WaiverRules;

        await _unitOfWork.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var fee = await _unitOfWork.Context.Set<ApplicationFee>().FindAsync(id);
        if (fee == null) return NotFound();

        fee.IsActive = false;
        await _unitOfWork.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("by-job/{jobPostingId}")]
    public async Task<IActionResult> GetByJobPosting(long jobPostingId)
    {
        var fee = await _unitOfWork.Context.Set<ApplicationFee>()
            .Where(f => f.JobPostingId == jobPostingId && f.IsActive)
            .Select(f => new { f.ApplicationFeeId, f.Amount, f.Currency, f.PaymentMethods })
            .FirstOrDefaultAsync();

        return Ok(fee);
    }
}

public class ApplicationFeeRequest
{
    public long JobPostingId { get; set; }
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public string? PaymentMethods { get; set; }
    public string? WaiverRules { get; set; }
}
