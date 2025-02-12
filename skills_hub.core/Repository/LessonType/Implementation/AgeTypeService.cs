using skills_hub.core.Repository.LessonType.Interfaces;
using skills_hub.core.Validators;

namespace skills_hub.core.Repository.LessonType.Implementation;

//For Admin Panel
public class AgeTypeService : AbstractLessonTypeLogModelService<AgeType>
{
    public AgeTypeService(ApplicationDbContext context, ILessonTypeService lessonTypeService)
    {
        _context = context;
        _validator = new AgeTypeValidator();
        _contextModel = _context.AgeTypes;
        _lessonTypeService = lessonTypeService;
    }

    protected override void SetPropertyId(domain.Models.LessonTypes.LessonType item, Guid value)
    {
        item.AgeTypeId = value;
    }
}
