using AutoMapper;
using Microsoft.AspNetCore.Identity;
using skills_hub.core.Validators;

namespace skills_hub.core.Repository.User;

public class BaseUserInfoService : AbstractLogModelService<BaseUserInfo>
{
    private readonly ApplicationDbContext _context;


    public BaseUserInfoService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _validator = new BaseUserInfoValidator();
        _contextModel = _context.BaseUserInfo;
    }

}