using Microsoft.AspNetCore.Identity;
using skills_hub.core.Repository.LessonType.Interfaces;
using skills_hub.core.Validators;

namespace skills_hub.core.Repository.LessonType.Implementation;

//For Admin Panel
public class LocationService : AbstractLessonTypeLogModelService<Location>
{
    public LocationService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILessonTypeService lessonTypeService)
    {
        _context = context;
        _validator = new LocationValidator();
        _contextModel = _context.Locations;
        _lessonTypeService = lessonTypeService;
    }

    protected override void SetPropertyId(domain.Models.LessonTypes.LessonType item, Guid value)
    {
        item.LocationId = value;
    }
}
