using System.ComponentModel.DataAnnotations.Schema;

namespace skills_hub.domain.Models.LessonTypes;

public class GroupType : LessonTypePropertyModel<GroupType>
{
    public int MinimumStudents { get; set; }
    public int MaximumStudents { get; set; }

    [NotMapped]
    public string? FullName
    {
        get
        {
            var res = Name;
            if (MinimumStudents == MaximumStudents) res += " (" + MinimumStudents + " students)";
            else res += " (" + MinimumStudents + " - " + MaximumStudents + " students)";
            return res;
        }
    }

    [NotMapped]
    public override string? DisplayName
    {
        get
        {
            var res = Name + " ";
            if (MinimumStudents == MaximumStudents) res += MinimumStudents + " студента(ов)";
            else res += " " + MinimumStudents + " - " + MaximumStudents + " студента(ов)";
            return res;
        }
    }


    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        GroupType other = (GroupType)obj;
        return Name == other.Name ||
               MinimumStudents == other.MinimumStudents && MaximumStudents == other.MaximumStudents;
    }
}