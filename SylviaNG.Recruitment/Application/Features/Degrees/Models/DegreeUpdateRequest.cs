namespace SylviaNG.Recruitment.Application.Features.Degrees.Models
{
    public class DegreeUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int Position { get; set; }
    }
}
