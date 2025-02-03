using skills_hub.domain.Models.Groups;
using skills_hub.domain.Models.User;

namespace skills_hub.domain.Models.ManyToMany;

public class LessonTeacher : LogModel<LessonTeacher>
{
    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; }
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; }

    public override bool Equals(object obj)
    {
        return false;
    }
}