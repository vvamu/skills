using skills_hub.domain.Base;
using skills_hub.domain.Models.Groups;
using skills_hub.domain.Models.ManyToMany;
using System.ComponentModel.DataAnnotations.Schema;

namespace skills_hub.domain.Models.User;

public class Student : BaseEntity
{
    public ApplicationUser ApplicationUser { get; set; }
    public Guid ApplicationUserId { get; set; }
    public List<GroupStudent>? Groups { get; set; }
    public List<LessonStudent>? Lessons { get; set; }
    public string? WorkingDays { get; set; }

    public int CountPayedLessons { get; set; }

    #region NotMapped


    [NotMapped]
    public decimal CurrentCalculatedPrice { get; set; }


    [NotMapped]
    public decimal TotalCalculatedPrice { get; set; }

    [NotMapped]
    public List<Lesson> VisitedLessons
    {
        get
        {
            List<Lesson> lessons = new List<Lesson>();
            if (Groups == null || Groups.Select(x => x.Group).Where(x => x.Lessons != null).Count() == 0) return lessons;
            lessons = Groups.Select(x => x.Group).Where(x => x.Lessons != null).SelectMany(x => x.Lessons).Where(x => x.IsСompleted).Where(x => x.ArrivedStudents != null && x.ArrivedStudents.Select(x => x.Student.Id).Contains(this.Id)).ToList();

            return lessons;
        }
    }

    [NotMapped]
    public List<Lesson> PreparedLessons
    {
        get
        {
            List<Lesson> lessons = new List<Lesson>();
            if (Groups == null || Groups.Select(x => x.Group).Where(x => x.Lessons != null).Count() == 0) return lessons;
            lessons = Groups.Select(x => x.Group).Where(x => x.Lessons != null).SelectMany(x => x.Lessons).Where(x => x.IsСompleted).Where(x => x.ArrivedStudents != null && x.ArrivedStudents.Select(x => x.Student.Id).Contains(this.Id)).ToList();
            return lessons;
        }
    }

    #endregion





}
//?