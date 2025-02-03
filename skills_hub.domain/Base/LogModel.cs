using skills_hub.domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace skills_hub.domain;

public abstract class LogModel<T> : BaseEntity
{
    public T? Parent { get; set; }
    public Guid? ParentId { get; set; }
    public string? Name { get; set; }
    public DateTime? DateRegistration { get; set; }

    [NotMapped]
    public virtual string? DisplayName { get; }

    public abstract bool Equals(object obj);

    [NotMapped]
    public List<T>? Parents { get; set; }

    [NotMapped]
    public List<T>? Children { get; set; }

}
