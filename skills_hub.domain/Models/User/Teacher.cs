using skills_hub.domain.Base;
using skills_hub.domain.Models.ManyToMany;
using System.ComponentModel.DataAnnotations.Schema;

namespace skills_hub.domain.Models.User;

public class Teacher : BaseEntity
{
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }

    public List<GroupTeacher>? Groups { get; set; }
    public List<LessonTeacher>? Lessons { get; set; }

    public string? WorkingDays { get; set; }
    public decimal PaidAmount { get; set; }


    #region NotMapped

    [NotMapped]
    public decimal CurrentCalculatedPrice { get; set; }
    [NotMapped]
    public decimal TotalCalculatedPrice { get; set; }



    #endregion
}