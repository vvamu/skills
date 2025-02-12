using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using skills_hub.core.Validators;

namespace skills_hub.core.Repository.User;

public class StudentService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    public StudentService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
    }
    public IQueryable<Student> GetItems()
    {
        var items = _context.Students
            .Include(x => x.ApplicationUser).ThenInclude(x => x.UserTeacher)
            .Include(x => x.Lessons).ThenInclude(x => x.Lesson)
            .Include(x => x.Groups).ThenInclude(x => x.Group).ThenInclude(x => x.Lessons)
            .OrderBy(on => on.Id);

        return items;
    }

    public async Task<Student> CreateStudentAsync(ApplicationUser user, Student item)
    {
        var dbUser = user;
        if (dbUser == null) dbUser = item.ApplicationUser;
        if (dbUser == null) dbUser = await _context.Users.FindAsync(item.ApplicationUserId);

        var userRegisterValidator = new StudentValidator();
        var validationResult = await userRegisterValidator.ValidateAsync(item);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors;
            var errorsString = string.Concat(errors);
            throw new Exception(errorsString);
        }

        if (item.ApplicationUserId != Guid.Empty)
        {
            await _context.Students.AddAsync(item);
            await _context.SaveChangesAsync();

            var res = await _userManager.AddToRoleAsync(dbUser, "Student");
            await _context.SaveChangesAsync();
            if (!res.Succeeded) throw new Exception(res.Errors.ToString());
            return item == null ? throw new CannotUnloadAppDomainException() : item;


        }


        ////---------------------------
        var student = _mapper.Map<Student>(item);
        student.ApplicationUser = dbUser;


        dbUser = item.ApplicationUser;

        dbUser.UserStudent = item;


        _context.Entry(student.ApplicationUser).State = EntityState.Unchanged;
        _context.Entry(dbUser.UserStudent).State = EntityState.Unchanged;

        _context.Users.Update(dbUser);
        await _context.Students.AddAsync(student);
        var result = await _userManager.AddToRoleAsync(dbUser, "Student");
        await _context.SaveChangesAsync();
        if (!result.Succeeded) throw new Exception(result.Errors.ToString());


        return student == null ? throw new CannotUnloadAppDomainException() : student;
    }

    public async Task<Student> DeleteStudentAsync(Guid id)
    {
        var dbItem = await _context.Students.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.Id == id);
        var user = dbItem.ApplicationUser;
        dbItem.IsDeleted = true;
        await _userManager.RemoveFromRoleAsync(user, "Student");
        await _context.SaveChangesAsync();
        var student = _mapper.Map<Student>(dbItem);
        return student;
    }
}