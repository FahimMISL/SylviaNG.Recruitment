using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Admin-managed lookup of countries (ISO alpha-2 code + international dial code), used to power
/// the candidate Contact section's country-code dropdown (e.g. "BD +880") in front of the local
/// mobile number. Also usable anywhere else a country picker is needed later.
/// </summary>
public class Country : Audit
{
    public long CountryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string DialCode { get; set; } = string.Empty;
}
