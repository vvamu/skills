using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using skills_hub.core.DTO;
using skills_hub.core.Validators;
using skills_hub.domain.Models.ManyToMany;

namespace skills_hub.core.Repository.User;

public class UserRepository : AbstractTransactionService
{
    protected readonly SignInManager<ApplicationUser> _signInManager;
    protected readonly UserManager<ApplicationUser> _userManager;
    //private readonly RoleManager<ApplicationUser> _roleManager;
    protected readonly ApplicationDbContext _context;
    protected readonly IQueryable<ApplicationUser> _fullInclude;
    protected readonly IMapper _mapper;
    protected readonly IAbstractLogModel<BaseUserInfo> _baseUserInfoService;

    public UserRepository(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
        ApplicationDbContext context, IMapper mapper, IAbstractLogModel<BaseUserInfo> baseUserInfoService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        //_roleManager = roleManager;
        _context = context;
        _mapper = mapper;
        _baseUserInfoService = baseUserInfoService;
        _fullInclude = _context?.Users?.AsNoTracking()
            .Include(x => x.UserTeacher)//.ThenInclude(x => x.PossibleCources).ThenInclude(x => x.LessonType)
            .Include(x => x.UserTeacher).ThenInclude(x => x.Groups)
            //.Include(x => x.UserTeacher).ThenInclude(x => x.PossibleCources)
            .Include(x => x.UserStudent).ThenInclude(x => x.Groups).ThenInclude(x => x.Group).ThenInclude(x => x.Lessons)//.ThenInclude(x=>x.GroupStudents).ThenInclude(x => x.Student)
            .Include(x => x.UserStudent).ThenInclude(x => x.Lessons)
            //.Include(x => x.UserStudent).ThenInclude(x => x.PossibleCources).ThenInclude(x => x.LessonType)
            .Include(x => x.ConnectedUsersInfo).ThenInclude(x => x.BaseUserInfo)
           //.Include(x => x.UserWorkingDays)

           ;
        

    }

    public async Task<ApplicationUser> GetAsync(Guid id)
    {
        var user = await _fullInclude.FirstOrDefaultAsync(x => x.Id == id);
        _context.ChangeTracker.Clear();
        if (user == null) return null;

        try
        {
            _context.Users.Attach(user);
            _context.Entry(user).State = EntityState.Detached;
        }
        catch (Exception ex) { }

        return user;
    }
    public async Task<IQueryable<ApplicationUser>> GetItems()
    {
        var users = _fullInclude.OrderBy(x => x.Id);
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            user.Roles = string.Join(";", roles);
        }
        return users;

    }

    protected async Task<ApplicationUser> CreateAsync(UserCreateDTO item)
    {
        var user = _mapper.Map<ApplicationUser>(item);
        var userInfo = _mapper.Map<BaseUserInfo>(item);

        #region Validators
        var userRegisterValidator = new UserCreateDTOValidator();
        var userValidationResult = await userRegisterValidator.ValidateAsync(item);
        if (!userValidationResult.IsValid)
        {
            var errors = userValidationResult.Errors;
            var errorsString = string.Concat(errors);
            throw new Exception(errorsString);
        }
        if (_context.Users.FirstOrDefault(x => x.UserName == user.UserName) != null) throw new Exception("User with this login already exists");
        #endregion

        string hashedPassword = HashProvider.ComputeHash(item.Password.Trim());
        user.OwnHashedPassword = hashedPassword; //

        var result = await _context.Users.AddAsync(user);
        await _userManager.UpdateSecurityStampAsync(user);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<ApplicationUser> UpdateAsync(UserCreateDTO item)
    {
        #region CreateUser
        var dbUser = await GetAsync(item.Id);

        if (dbUser == null) throw new Exception("User was not found in database");

        var connectedUserInfo = dbUser;
        var user = _mapper.Map<ApplicationUser>(item);
        #region Validators
        var userRegisterValidator = new UserCreateDTOValidator();
        var userValidationResult = await userRegisterValidator.ValidateAsync(item);
        if (!userValidationResult.IsValid)
        {
            var errors = userValidationResult.Errors;
            var errorsString = string.Concat(errors);
            throw new Exception(errorsString);
        }
        if (_context.Users.FirstOrDefault(x => x.UserName == user.UserName) != null) throw new Exception("User with this login already exists");
        #endregion
        user.OwnHashedPassword = HashProvider.ComputeHash(item.Password.Trim());


        if (!HashProvider.VerifyHash(item.Password.Trim(), dbUser.OwnHashedPassword)) throw new Exception("Password not equal");
        if (!string.IsNullOrEmpty(item.PasswordChanged) && !string.IsNullOrEmpty(item.PasswordChangedConfirm))
        {
            if (item.PasswordChanged != item.PasswordChangedConfirm) throw new Exception("Changed password values ​​are not equal");
            user.OwnHashedPassword = HashProvider.ComputeHash(item.PasswordChangedConfirm.Trim());
        }
        dbUser = user;
        var result = _context.Users.Update(dbUser);
        var saved = false;
        // SaveChanges
        while (!saved)
        {
            try
            {
                _context.SaveChanges();
                saved = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is ApplicationUser)
                    {
                        var proposedValues = entry.CurrentValues;
                        var databaseValues = entry.GetDatabaseValues();

                        foreach (var property in proposedValues.Properties)
                        {
                            var proposedValue = proposedValues[property];
                            var databaseValue = databaseValues[property];

                            if (proposedValue != null && !proposedValue.Equals(databaseValue))
                            {
                                proposedValues[property] = proposedValue; 
                            }
                        }
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                    else
                    {
                        throw new NotSupportedException(
                            "Don't know how to handle concurrency conflicts for "
                            + entry.Metadata.Name);
                    }
                }
            }
        }

        #endregion

       
        return user;

    }

    #region Delete

    public async Task<ApplicationUser> DeleteAsync(ApplicationUser item, bool isHardDelete = false)
    {

        if (item == null) throw new Exception("User not found");
        if (!item.IsDeleted || isHardDelete)
            await SoftDeleteAsync(item);


        var executionStrategy = CreateExecutionStrategy();
        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await BeginTransactionAsync();
            try
            {
                _context.Users.Remove(item);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await RollbackAsync(transaction);
                throw new Exception("Не получилось обновить элемент. Попробуйте позже.", ex);
            }
        });


        return item;
    }

    protected virtual async Task<ApplicationUser> SoftDeleteAsync(ApplicationUser item)
    {
        var dbItem = await _context.Users.FirstOrDefaultAsync(x => x.Id == item.Id) ?? throw new Exception("User not found");
        dbItem.IsDeleted = true;
        await _context.SaveChangesAsync();
        return dbItem;
  
    }

    public async Task<ApplicationUser> Restore(ApplicationUser item)
    {
        item.IsDeleted = false;
        _context.Users.Update(item);
        await _context.SaveChangesAsync();
        return item;
    }

    #endregion


}
