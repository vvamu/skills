using skills_hub.domain.Base;
using skills_hub.domain.Models.User;

namespace skills_hub.domain.Models.ManyToMany;

public class NotificationUser : BaseEntity
{
    public NotificationMessage NotificationMessage { get; set; }
    public Guid NotificationMessageId { get; set; }
    public ApplicationUser User { get; set; }
    public Guid UserId { get; set; }
}
