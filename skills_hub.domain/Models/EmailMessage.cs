using skills_hub.domain.Base;
using skills_hub.domain.Models.User;
using System.ComponentModel.DataAnnotations;

namespace skills_hub.domain.Models;

public class EmailMessage : BaseEntity
{
    [Required]
    public string Name { get; set; }
    [Required]
    [Phone]
    public string? Phone { get; set; }

    [Required]
    public string? Data { get; set; }

    public string Date { get; set; } = DateTime.Now.ToString();

    [EmailAddress]
    public string? Email { get; set; } = "-";

    public ApplicationUser? Sender { get; set; }
    public Guid? SenderId { get; set; }

    public DateTime DateCreated { get; set; }



}
