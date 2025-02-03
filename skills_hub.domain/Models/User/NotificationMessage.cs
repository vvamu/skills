using skills_hub.domain.Base;
using skills_hub.domain.Models.ManyToMany;

namespace skills_hub.domain.Models.User;

public class NotificationMessage : BaseEntity
{
    public string Message { get; set; }
    public List<NotificationUser>? Users { get; set; }

    public bool IsRequest { get; set; }
    public ApplicationUser? Sender { get; set; }
}
