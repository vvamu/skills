using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace skills_hub.core.Repository.LessonType;

//For Admin Panel
public class AgeTypeServices : IAgeTypeServices
{
    private ApplicationDbContext _context;
    private UserManager<ApplicationUser> _userManager;
    private RoleManager<IdentityRole<Guid>> _roleManager;

    public AgeTypeServices(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<List<AgeType>> GetAllAsync()
    {

        return await _context.AgeTypes.ToListAsync();
    }
    public async Task<AgeType> CreateAsync()
    {
        var res = await _context.AgeTypes.AddAsync(new AgeType() { MinimumAge = 1, MaximumAge = 20, Name = "Testing value" });
        _context.SaveChanges();



        return res.Entity;
    }

    public async Task<ApplicationUser> CreateUserAsync()
    {
        var user = new ApplicationUser()
        {
            Login = "test_login",
            UserName = "test_name",
            OwnHashedPassword = "23456",
        };
        var res = await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        //var resUser = res.Entity; // Получаем добавленного пользователя

        //await _userManager.AddToRoleAsync(resUser, "Teacher");
        //await _context.SaveChangesAsync();
        //var userRoles = await _userManager.IsInRoleAsync(resUser,"Student");
        //var userRoles2 = await _userManager.IsInRoleAsync(resUser, "Teacher");

        return res.Entity;

    }

    public async Task<List<ApplicationUser>> GetUsersAsync()
    {
        var resUser2 = await _context.Users.ToListAsync();
        if (resUser2 != null && resUser2.Count > 0)
        {
            var userm = await _userManager.GetRolesAsync(resUser2.First());
            var u = userm.ToList();
        }


        return resUser2;

    }
}

public interface IAgeTypeServices
{
    public Task<List<AgeType>> GetAllAsync();
    public Task<AgeType> CreateAsync();
}