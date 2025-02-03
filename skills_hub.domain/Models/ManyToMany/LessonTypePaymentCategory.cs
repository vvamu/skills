using skills_hub.domain.Base;
using skills_hub.domain.Models.LessonTypes;

namespace skills_hub.domain.Models.ManyToMany;

public class LessonTypePaymentCategory : BaseEntity
{
    public PaymentCategory PaymentCategory { get; set; }
    public Guid? PaymentCategoryId { get; set; }
    public LessonType LessonType { get; set; }
    public Guid LessonTypeId { get; set; }
    public DateTime DateAdd { get; set; }

}