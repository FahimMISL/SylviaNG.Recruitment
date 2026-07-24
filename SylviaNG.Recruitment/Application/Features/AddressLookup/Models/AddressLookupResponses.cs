namespace SylviaNG.Recruitment.Application.Features.AddressLookup.Models
{
    public class DivisionResponse
    {
        public long DivisionId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class DistrictResponse
    {
        public long DistrictId { get; set; }
        public string Name { get; set; } = string.Empty;
        public long DivisionId { get; set; }
    }

    public class ThanaResponse
    {
        public long ThanaId { get; set; }
        public string Name { get; set; } = string.Empty;
        public long DistrictId { get; set; }
    }
}
