using Microsoft.AspNetCore.Identity;
using skills_hub.core.Repository.LessonType.Interfaces;
using skills_hub.core.Validators;

namespace skills_hub.core.Repository.LessonType;

//For Admin Panel
public class AgeTypeService : AbstractLessonTypeLogModelService<AgeType>
{
    public AgeTypeService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILessonTypeService lessonTypeService)
    {
        _context = context;
        _validator = new AgeTypeValidator();
        _contextModel = _context.AgeTypes;
        _lessonTypeService = lessonTypeService;
    }

    protected override void SetPropertyId(skills_hub.domain.Models.LessonTypes.LessonType item, Guid value)
    {
        item.AgeTypeId = value;
    }
}
