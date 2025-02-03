using skills_hub.domain.Models.LessonTypes;

namespace skills_hub.domain;

public abstract class LessonTypePropertyModel<T> : LogModel<T>
{
    public List<LessonType>? LessonTypes { get; set; }

}
