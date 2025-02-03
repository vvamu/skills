using skills_hub.domain.Models.Groups;
using skills_hub.domain.Models.User;

namespace skills_hub.domain.Models.ManyToMany;

public class GroupStudent : LogModel<GroupStudent>
{
    public Guid GroupId { get; set; }
    public Group Group { get; set; }
    public Guid StudentId { get; set; }
    public Student Student { get; set; }
    public DateTime DateAdd { get; set; } = DateTime.Now;

    public override bool Equals(object obj)
    {
        return false;
    }
}
