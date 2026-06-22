namespace SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Models
{
    public class ShortlistFilterCriteriaCreateRequest
    {
        public long ShortlistFilterId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool IsHardFilter { get; set; }
        public int LayerOrder { get; set; }
    }
}
