using System.ComponentModel.DataAnnotations.Schema;

namespace skills_hub.domain.Models.LessonTypes;

public class Location : LessonTypePropertyModel<Location>
{
    public bool IsOffline { get; set; }
    public string? Description { get; set; }

    [NotMapped]
    public string IsOfflineTextRu { get => IsOffline ? "Оффлайн" : "Онлайн"; }


    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Location other = (Location)obj;
        return Name == other.Name && Status == other.Status && Description == other.Description;
    }

    [NotMapped]
    public override string DisplayName => $"{IsOfflineTextRu} - {Name} " + Description;
}