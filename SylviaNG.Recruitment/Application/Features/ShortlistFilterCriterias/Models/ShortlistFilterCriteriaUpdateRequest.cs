namespace SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Models
{
    public class ShortlistFilterCriteriaUpdateRequest
    {
        public long? ShortlistFilterId { get; set; }
        public string? FieldName { get; set; }
        public string? Operator { get; set; }
        public string? Value { get; set; }
        public bool? IsHardFilter { get; set; }
        public int? LayerOrder { get; set; }
        public bool? IsActive { get; set; }
    }
}
