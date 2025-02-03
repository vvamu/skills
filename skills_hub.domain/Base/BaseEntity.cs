using System.ComponentModel.DataAnnotations.Schema;

namespace skills_hub.domain.Base;

public class BaseEntity
{
    public Guid Id { get; set; }

    public DateTime DateCreated { get; set; } = DateTime.Now;

    public bool IsDeleted { get; set; } = false;

    [NotMapped]
    public string? Status { get => IsDeleted ? "Удален" : "Активен"; }

}
