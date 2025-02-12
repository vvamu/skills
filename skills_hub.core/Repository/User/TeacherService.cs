using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using skills_hub.core.Validators;

namespace skills_hub.core.Repository.User;

public class TeacherService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public TeacherService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
    }

    public IQueryable<Teacher> GetItems()
    {
        var items = _context.Teachers
            .Include(x => x.ApplicationUser)
            .OrderBy(on => on.Id);

        return items;

    }
    public async Task<Teacher> CreateAsync(ApplicationUser user, Teacher item)
    {
        //var dbUser = await _context.Users.Where(x => x.Id == userId).FirstOrDefaultAsync() ?? throw new Exception("User not found");
        var dbUser = user;
        if (user == null) dbUser = item.ApplicationUser;
        var userRegisterValidator = new Validators.TeacherValidator();

        var validationResult = await userRegisterValidator.ValidateAsync(item);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors;
            var errorsString = string.Concat(errors);
            throw new Exception(errorsString);
        }

        if (item.ApplicationUserId != Guid.Empty)
        {
            await _context.Teachers.AddAsync(item);
            await _context.SaveChangesAsync();

            var result = await _userManager.AddToRoleAsync(dbUser, "Teacher");
            if (!result.Succeeded) throw new Exception(result.Errors.ToString());
            await _context.SaveChangesAsync();

            return item == null ? throw new Exception("Error with save teacher in database") : item;
        }
        return item;

        /*


        _context.Entry(item.ApplicationUser).State = EntityState.Unchanged;
        _context.Entry(dbUser.UserTeacher).State = EntityState.Unchanged;
        //if (!result.Succeeded) throw new Exception(result.Errors.ToString());
        */

    }

    public async Task<Teacher> UpdateTeacherAsync(Teacher item)
    {
        //if (_context.Teachers.Any(x => x.Email == item.Email)) throw new Exception("Teacher with such email alredy exists");
        var teacher = await _context.Teachers.FirstOrDefaultAsync(x => x.Id == item.Id) ?? throw new Exception("Teacher not found");
        var userRegisterValidator = new TeacherValidator();
        var validationResult = await userRegisterValidator.ValidateAsync(teacher);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors;
            var errorsString = string.Concat(errors);
            throw new Exception(errorsString);
        }

        _context.Teachers.Update(item);
        await _context.SaveChangesAsync();

        return item == null ? throw new CannotUnloadAppDomainException() : item;

    }

    public async Task<Teacher> DeleteTeacherAsync(Guid id)
    {
        var dbItem = await _context.Teachers.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.Id == id);
        var user = dbItem.ApplicationUser;
        dbItem.IsDeleted = true;
        await _userManager.RemoveFromRoleAsync(user, "Teacher");
        await _context.SaveChangesAsync();
        var teacher = _mapper.Map<Teacher>(dbItem);
        return teacher;
    }
}