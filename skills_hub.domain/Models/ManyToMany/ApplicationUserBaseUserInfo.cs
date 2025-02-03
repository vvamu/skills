using skills_hub.domain.Base;
using skills_hub.domain.Models.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace skills_hub.domain.Models.ManyToMany;

public class ApplicationUserBaseUserInfo : BaseEntity
{
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }


    public BaseUserInfo BaseUserInfo { get; set; }
    public Guid BaseUserInfoId { get; set; }

    public bool IsBase { get; set; }
    public string? Role { get; set; }

    [NotMapped]
    public string? UserAndRole { get; set; }


}