using skills_hub.core.Validators;

namespace skills_hub.core.Repository.User;

public class BaseUserInfoService : AbstractLogModelService<BaseUserInfo>
{
    public BaseUserInfoService(ApplicationDbContext context)
    {
        _context = context;
        _contextModel = _context.BaseUserInfo;
        _validator = new BaseUserInfoValidator();

    }

}