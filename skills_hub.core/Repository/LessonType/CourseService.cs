using Microsoft.AspNetCore.Identity;
using skills_hub.core.Repository.LessonType.Interfaces;
using SkillsHub.Application.Validators.LessonTypeModels;

namespace skills_hub.core.Repository.LessonType;

//For Admin Panel
public class CourseService : AbstractLessonTypeLogModelService<Course>
{
    public CourseService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILessonTypeService lessonTypeService)
    {
        _context = context;
        _validator = new CourseValidator();
        _contextModel = _context.Courses;
        _lessonTypeService = lessonTypeService;
    }

    protected override void SetPropertyId(skills_hub.domain.Models.LessonTypes.LessonType item, Guid value)
    {
        item.CourseId = value;
    }
}
