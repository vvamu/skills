using Microsoft.EntityFrameworkCore;
using skills_hub.core.Repository.LessonType.Interfaces;
using skills_hub.core.Repository.User.Interfaces;
using skills_hub.domain.Models.ManyToMany;
using SkillsHub.Application.Validators.LessonTypeModels;

namespace skills_hub.core.Repository.LessonType.Implementation;
using LessonType = domain.Models.LessonTypes.LessonType;

//For Admin Panel
public class LessonTypeService : AbstractLogModelService<LessonType>, ILessonTypeService
{
    private readonly INotificationService? _notificationService;

    public LessonTypeService(ApplicationDbContext context, INotificationService notificationService = null)
    {
        _context = context;
        _contextModel = _context.LessonTypes;
        _validator = new LessonTypeValidator();
        _notificationService = notificationService;
        _fullInclude = _contextModel
            .Include(x => x.Course)
            .Include(x => x.LessonTypePaymentCategory).ThenInclude(x => x.PaymentCategory)
            .Include(x => x.GroupType)
            .Include(x => x.Location)
            .Include(x => x.AgeType);
    }

    public override async Task<LessonType>? GetLastValueAsync(Guid? itemId, bool withParents = false)
    {
        var item = await _fullInclude.FirstOrDefaultAsync(x => x.Id == itemId);
        if (!withParents) return item ?? throw new Exception("Not found");

        //item.Children = GetAllParents((Guid)item.Id).Where(x => x.Id != item.Id).ToList();
        item.Parents = GetAllChildren(item.Id).Where(x => x.Id != item.Id).ToList();

        for (int i = 0; i < item.Parents.Count; i++)
        {
            item.Parents[i] = await _fullInclude.FirstOrDefaultAsync(x => x.Id == item.Parents[i].Id);
        }

        return item ?? throw new Exception("Not found");
    }

    public async Task<LessonType> CreateAsync(LessonType item, Guid[] paymentCategories)
    {
        var payments = await CheckCorrectPaymentCategories(paymentCategories, item.IsActive);
        var res = await CreateAsync(item);
        var lessonTypePayments = payments.Select(x => new LessonTypePaymentCategory() { PaymentCategoryId = x.Id, LessonTypeId = item.Id });
        await _context.LessonTypePaymentCategories.AddRangeAsync(lessonTypePayments);
        await _context.SaveChangesAsync();
        /*
        var durationName =
            {
                get => DurationTypeValue;
        set
        {
            DurationTypeValue = value;
            if (!string.IsNullOrEmpty(DurationTypeName) && DurationTypeName == )
            {
                switch (DurationTypeName)
                {
                    case "lesson": { CountWorkingHours = DurationTypeValue * LessonTimeInMinutes; break; }
                    case "month": { CountWorkingHours = DurationTypeValue * LessonTimeInMinutes; break; }


                }
                Count
                    }
        }
    }*/

        return res;

    }
    public async Task<LessonType> UpdateAsync(LessonType item, Guid[] paymentCategories)
    {
        var olItemDb = await GetAsync(item.Id) ?? throw new Exception("Lesson type not found");
        var payments = await CheckCorrectPaymentCategories(paymentCategories, item.IsActive);

        if (!AreObjectsDifferent(olItemDb, item)) return item;
        
        await CheckCorrectActiveProperties(item.AgeTypeId, item.CourseId, item.GroupTypeId, item.LocationId);
        item = await base.UpdateAsync(item);
        return item;
        /*
        var oldLessonTypeStudents = await _context.LessonTypeStudents.Where(x => x.LessonTypeId == item.Id).ToListAsync();
        var oldLessonTypeTeachers = await _context.LessonTypeTeachers.Where(x => x.LessonTypeId == item.Id).ToListAsync();

        //var itemDb = await GetAsync(item.Id);

        var lessonTypePayments = payments.Select(x => new LessonTypePaymentCategory() { PaymentCategoryId = x.Id, LessonTypeId = item.Id });
        var lessonTypeStudents = oldLessonTypeStudents.Select(x => new LessonTypeStudent() { StudentId = x.StudentId, LessonTypeId = item.Id });
        var lessonTypeTeachers = oldLessonTypeTeachers.Select(x => new LessonTypeTeacher() { TeacherId = x.TeacherId, LessonTypeId = item.Id });

        await _context.LessonTypePaymentCategories.AddRangeAsync(lessonTypePayments);
        await _context.SaveChangesAsync();
        await _context.LessonTypeStudents.AddRangeAsync(lessonTypeStudents);
        await _context.SaveChangesAsync();
        await _context.LessonTypeTeachers.AddRangeAsync(lessonTypeTeachers);
        await _context.SaveChangesAsync();

        var oldPayemntCatogories = olItemDb?.LessonTypePaymentCategory?.Where(x=>x.PaymentCategoryId != null).Select(x=>x.PaymentCategoryId ?? Guid.Empty).ToList() ?? new List<Guid>(); 
        var toUpdate = paymentCategories.Intersect(oldPayemntCatogories).ToList();
        var toCreate = paymentCategories.Except(oldPayemntCatogories).ToList();
        var toDelete = oldPayemntCatogories.Except(paymentCategories).ToList();
        
        foreach (var it in toDelete)
        {
            //var del = await _context.LessonTypePaymentCategories.FirstOrDefaultAsync(x=>x.PaymentCategoryId == it &&  x.LessonTypeId == item.Id);
            //_context.LessonTypePaymentCategories.Remove(del);
        }
        foreach (var it in toUpdate)
        {
            
            if (!AreObjectsDifferent(olItemDb, item)) break;
            var update = await _context.LessonTypePaymentCategories.FirstOrDefaultAsync(x => x.PaymentCategoryId == it && x.LessonTypeId == item.Id);
            update.LessonTypeId = item.Id;
            _context.LessonTypePaymentCategories.Update(update);
            
        }
        await _context.SaveChangesAsync();
        */




    }

    public override async Task<bool> IsHardDelete(IQueryable<LessonType> items)
    {
        //var oo = _contextModel.AsQueryable();

        //var itemsWithRefs = await items.Include(x=>x.Groups).Where(x=>x.Groups != null && x.Groups.Count() > 0).ToListAsync();
        var children = items.AsEnumerable().ToList();

        for (int i = 0; i < children.Count; i++)
        {
            children[i] = await _context.LessonTypes
                /*.Include(x => x.Course)
                .Include(x => x.LessonTypePaymentCategory).ThenInclude(x => x.PaymentCategory)
                .Include(x => x.GroupType)
                .Include(x => x.Location)
                .Include(x => x.AgeType*/
                .Include(x => x.Groups)
                //.Include(x => x.LessonTypeStudents)
                //.Include(x => x.LessonTypeTeachers)
                .FirstOrDefaultAsync(x => x.Id == children[i].Id);
            if (children[i] != null && children[i].Groups != null && children[i].Groups.Count() > 0) return false;
            //if (children[i] != null && children[i].LessonTypeStudents != null && children[i].LessonTypeStudents.Count() > 0) return false;
            //if (children[i] != null && children[i].LessonTypeTeachers != null && children[i].LessonTypeTeachers.Count() > 0) return false;
        }



        //var itemssss = _context.LessonTyp


        /*var itemsWithRefs = lessonTypes.Where(x => x.AgeTypeId != null
        || x.CourseId != null
        || x.GroupTypeId != null
        || x.LocationId != null
        || x.PaymentCategories != null).ToList();*/

        //if (itemsWithRefs == null || itemsWithRefs.Count() == 0) return true; //throw new Exception("There is a reference to the lesson type. Please remove the reference before deleting the item");




        return true;

    }

    public async Task<List<PaymentCategory>> CheckCorrectPaymentCategories(Guid[] paymentCategories, bool itemIsActive)
    {
        List<PaymentCategory> result = new List<PaymentCategory>();
        if (paymentCategories.Length == 0 && itemIsActive) throw new Exception("Before create active lesson type choose payment category before it");
        foreach (var it in paymentCategories)
        {
            var r = await _context.PaymentCategories.FirstOrDefaultAsync(x => x.Id == it);
            if (r == null) throw new Exception("Payment category not found");
            if (r.IsDeleted) throw new Exception($"{it} is deleted");
            result.Add(r);

        }
        return result;
    }

    public async Task CheckCorrectActiveProperties(Guid? ageTypeId, Guid? courseId, Guid? groupTypeId, Guid? locationId)
    {
        var isFound = _context.AgeTypes.FirstOrDefault(x => x.Id == ageTypeId && !x.IsDeleted) != null ? true : throw new Exception("Age type now was deleted");
        isFound = _context.Courses.FirstOrDefault(x => x.Id == courseId && !x.IsDeleted) != null ? true : throw new Exception("Course now was deleted");
        isFound = _context.GroupTypes.FirstOrDefault(x => x.Id == groupTypeId && !x.IsDeleted) != null ? true : throw new Exception("Group type now was deleted");
        isFound = _context.Locations.FirstOrDefault(x => x.Id == locationId && !x.IsDeleted) != null ? true : throw new Exception("Location type now was deleted");

    }


}
