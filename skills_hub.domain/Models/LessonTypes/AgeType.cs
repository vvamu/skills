using System.ComponentModel.DataAnnotations.Schema;

namespace skills_hub.domain.Models.LessonTypes;

public class AgeType : LessonTypePropertyModel<AgeType>
{
    public int MinimumAge { get; set; }
    public int MaximumAge { get; set; }

    [NotMapped]
    public string? DisplayName
    {
        get
        {
            var res = Name;
            if (MinimumAge == MaximumAge) res += " (" + MinimumAge + " лет)";
            else res += " (" + MinimumAge + " - " + MaximumAge + " лет)";
            return res;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        AgeType other = (AgeType)obj;
        return Name == other.Name ||
               MinimumAge == other.MinimumAge &&
               MaximumAge == other.MaximumAge;
    }
}

