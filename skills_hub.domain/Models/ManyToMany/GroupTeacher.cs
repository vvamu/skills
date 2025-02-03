using skills_hub.domain.Models.Groups;
using skills_hub.domain.Models.User;
using System.ComponentModel;

namespace skills_hub.domain.Models.ManyToMany;

public class GroupTeacher : LogModel<GroupTeacher>
{
    public Guid GroupId { get; set; }
    public Group Group { get; set; }
    public Teacher Teacher { get; set; }
    public Guid TeacherId { get; set; }

    [DefaultValue("CONVERT(datetime, GETDATE())")]
    public DateTime DateAdd { get; set; } = DateTime.Now;

    public override bool Equals(object obj)
    {
        var other = obj as GroupTeacher;
        if (other == null) return false;
        return true;
    }
}