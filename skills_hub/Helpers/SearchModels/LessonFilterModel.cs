namespace skills_hub.Helpers.SearchModels;

public class LessonFilterModel
{
    public string? Topic { get; set; }
    public Guid TeacherId { get; set; }
    public Guid StudentId { get; set; }
    public Guid GroupId { get; set; }
    public string? Category { get; set; }
    public DateTime? MinDateCreated { get; set; }
}