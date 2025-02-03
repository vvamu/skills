using skills_hub.domain.Models.Groups;
using skills_hub.domain.Models.User;

namespace skills_hub.domain.Models.ManyToMany;

public class LessonStudent : LogModel<LessonStudent>
{
    public Lesson Lesson { get; set; }
    public Guid LessonId { get; set; }
    public Student Student { get; set; }
    public Guid StudentId { get; set; }
    public int VisitStatus { get; set; }  //1 - Visited 2 - Missed without saving lesson 3 - Missed  with saving lesson 
    public string LessonType { get; set; } = "Default"; //Отработка/Занятие/Групповое/Trial

    public string? Comment { get; set; }
    public string? CommentAboutTeacher { get; set; }
    public double Grade { get; set; }
    public string? VisitStatusTextRu
    {
        get
        {

            if (Lesson != null && Lesson.IsСompleted)
            {

                return VisitStatus switch
                {
                    1 => "Собирается посетить",
                    2 => "Пропустит без сохранения урока",
                    3 => "Пропустит с сохранением урока",
                    _ => "Неизвестный статус",
                };

            }
            return VisitStatus switch
            {
                1 => "Посетил",
                2 => "Пропустил без сохранения урока",
                3 => "Пропустил с сохранением урока",
                _ => "Неизвестный статус",
            };
        }
    }
    public override bool Equals(object obj)
    {
        return false;
    }
}

//public class GroupStudent : BaseEntity
//{
//    public Guid GroupId { get; set; }
//    public Group Group { get; set; }
//    public Guid StudentId { get; set; }
//    public Student Student { get; set; }
//    public bool IsVisit { get; set; }

//}
