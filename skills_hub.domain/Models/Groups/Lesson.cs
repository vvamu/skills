using skills_hub.domain.Models.ManyToMany;
using System.ComponentModel.DataAnnotations.Schema;

namespace skills_hub.domain.Models.Groups;

public class Lesson : LogModel<Lesson>
{
    public string? Topic { get; set; }
    //[Url]
    public string? LinkToWebinar { get; set; }

    public LessonTeacher? Teacher { get; set; }
    public Guid? TeacherId { get; set; }
    public List<LessonStudent>? ArrivedStudents { get; set; }
    public Group? Group { get; set; }
    public Guid? GroupId { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    /*

    public LessonType? LessonType { get; set; } //Group, Individual
    public Guid? LessonTypeId { get; set; } 
    */
    public string? Location { get; set; } //Online Offline


    //public LessonActivityType? LessonActivityType { get; set; } //Отработка Пробное убрать
    public Guid? LessonTaskId { get; set; }
    //public ApplicationUser? Creator { get; set; }
    //public Guid CreatorId { get; set; }

    public string? Comment { get; set; } = "";
    public string? AdditionalMaterials { get; set; } = "";
    public bool IsСompleted { get; set; } = false;
    public DateTime DateCompletedByTeacher { get; set; }


    public override bool Equals(object obj)
    {
        return false;
    }

    public Lesson Clone()
    {
        return (Lesson)MemberwiseClone();
    }


    //public bool IsVerified { get; set; }
    #region NotMapped

    public Dictionary<int, string> StudentVisitStatusesRuDictionary
    {
        get
        {
            return !IsСompleted
                ? new Dictionary<int, string>
                {
                { 1, "Собирается посетить" },
                { 2, "Пропустит без сохранения урока" },
                { 3, "Пропустит с сохранением урока" }
                }
                : new Dictionary<int, string>
                {
                { 1, "Посетил" },
                { 2, "Пропустил без сохранения урока" },
                { 3, "Пропустил с сохранением урока" }
                };
        }
    }

    public string? IsCompletedTextRu
    {
        get
        {
            var res = "завершено";
            if (IsСompleted) return res;
            else return "не завершено";
        }
    }

    [NotMapped]
    public int Duration
    {
        get
        {
            int resultMinutes = 0;
            return (int)(EndTime - StartTime).TotalMinutes;
        }
    }



    #endregion

}


