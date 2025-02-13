using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using skills_hub.core.DTO;
using skills_hub.core.Validators;
using skills_hub.domain.Models.ManyToMany;

namespace skills_hub.core.Repository.User;

public class UserService : AbstractTransactionService, IUserService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    //private readonly RoleManager<ApplicationUser> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly IQueryable<ApplicationUser> _fullInclude;
    private readonly IMapper _mapper;
    private readonly IAbstractLogModel<BaseUserInfo> _baseUserInfoService;

    public UserService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
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
        CreateAdminAsync();

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

        try
        {
            if (user.UserTeacher != null)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                if (!user.UserTeacher.IsDeleted) await _userManager.AddToRoleAsync(user, "Teacher");
                if (user.UserTeacher.IsDeleted) await _userManager.RemoveFromRoleAsync(user, "Teacher");
            }
            if (user.UserStudent != null)
            {
                var us = new ApplicationUser() { Id = user.Id };
                await _userManager.UpdateSecurityStampAsync(us);
                if (!user.UserStudent.IsDeleted) await _userManager.AddToRoleAsync(user, "Student");
                if (user.UserStudent.IsDeleted) await _userManager.RemoveFromRoleAsync(user, "Student");
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) { }


        return user;
    }
    public async Task<UserCreateDTO> GetCreateDTOByIdAsync(Guid id)
    {
        var user = await GetAsync(id);
        if (user == null) return null;
        var userCreateDTO = _mapper.Map<UserCreateDTO>(user);
        userCreateDTO.BaseUserInfoId = user.UserInfo.Id;
        if (user.UserTeacher != null) userCreateDTO.IsTeacher = true;
        if (user.UserStudent != null) userCreateDTO.IsStudent = true;

        return userCreateDTO;
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

    public async Task<ApplicationUser?> GetCurrentUserAsync()
    {
        var userName = _signInManager.Context.User.Identity.Name;
        if (userName == null) throw new Exception();
        var dbUser = await _userManager.FindByNameAsync(userName);

        return await GetAsync(dbUser.Id);

    }
    public async Task<IQueryable<NotificationMessage>> GetCurrentUserNotifications()
    {
        var user = await GetCurrentUserAsync();
        List<NotificationMessage> userNotification = new List<NotificationMessage>();

        var notifications = _context.NotificationUsers.Include(x => x.User)
            .Include(x => x.NotificationMessage)
            .Where(x => x.User.Id == user.Id).Select(x => x.NotificationMessage).OrderBy(x => x.DateCreated);

        return notifications.AsQueryable();
    }

    public async Task<ApplicationUser> CreateAsync(UserCreateDTO item)
    {
        var user = _mapper.Map<ApplicationUser>(item);
        var userInfo = _mapper.Map<BaseUserInfo>(item);
        user.UserName = user.Login;


        #region Validators
        var userRegisterValidator = new UserCreateDTOValidator();
        var userValidationResult = await userRegisterValidator.ValidateAsync(item);
        if (!userValidationResult.IsValid)
        {
            var errors = userValidationResult.Errors;
            var errorsString = string.Concat(errors);
            throw new Exception(errorsString);
        }
        if (_context.Users.FirstOrDefault(x => x.Login == user.Login) != null) throw new Exception("User with this login already exists");
        #endregion

        string hashedPassword = HashProvider.ComputeHash(item.Password.Trim());
        user.OwnHashedPassword = hashedPassword;

        var result = await _context.Users.AddAsync(user);
        await _userManager.UpdateSecurityStampAsync(user);

        await _context.SaveChangesAsync();


        if (item.IsStudent == true)
        {
            var st = new Student() { ApplicationUserId = result.Entity.Id, IsDeleted = false }; await _context.Students.AddAsync(st); await _context.SaveChangesAsync();

            try
            {
                await _userManager.AddToRoleAsync(result.Entity, "Student");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is ApplicationUser)
                    {
                        var proposedValues = entry.CurrentValues; // Предложенные значения
                        var databaseValues = entry.GetDatabaseValues(); // Значения из базы данных

                        foreach (var property in proposedValues.Properties)
                        {
                            var proposedValue = proposedValues[property];
                            var databaseValue = databaseValues[property];

                            // Решение о том, какое значение использовать
                            // Можно выбрать proposedValue, databaseValue или применить свою логику объединения значений

                            // Например, просто обновляем оригинальные значения до значений из базы данных
                            entry.OriginalValues.SetValues(proposedValues);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException("Concurrency conflict occurred for entity type: " + entry.Entity.GetType().Name);
                    }
                }

                // Повторно пытаемся сохранить изменения
                _context.SaveChanges();
            }
        }
        if (item.IsTeacher == true)
        {
            var tch = new Teacher() { ApplicationUserId = result.Entity.Id, IsDeleted = false }; await _context.Teachers.AddAsync(tch); await _context.SaveChangesAsync();


            try
            {

                await _userManager.AddToRoleAsync(result.Entity, "Teacher");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is ApplicationUser)
                    {
                        var proposedValues = entry.CurrentValues; // Предложенные значения
                        var databaseValues = entry.GetDatabaseValues(); // Значения из базы данных

                        foreach (var property in proposedValues.Properties)
                        {
                            var proposedValue = proposedValues[property];
                            var databaseValue = databaseValues[property];

                            // Решение о том, какое значение использовать
                            // Можно выбрать proposedValue, databaseValue или применить свою логику объединения значений

                            // Например, просто обновляем оригинальные значения до значений из базы данных
                            entry.OriginalValues.SetValues(proposedValues);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException("Concurrency conflict occurred for entity type: " + entry.Entity.GetType().Name);
                    }
                }

                // Повторно пытаемся сохранить изменения
                _context.SaveChanges();
            }
        }
        return await GetAsync(result.Entity.Id);
    }

    public async Task<ApplicationUser> UpdateAsync(UserCreateDTO item)
    {
        #region CreateUser
        var dbUser = await GetAsync(item.Id);

        if (dbUser == null) throw new Exception("User was not found in database");

        var connectedUserInfo = dbUser;
        var user = _mapper.Map<ApplicationUser>(item);
        user.UserName = user.Login;
        user.OwnHashedPassword = HashProvider.ComputeHash(item.Password.Trim());


        #region Validators
        var userRegisterValidator = new UserCreateDTOValidator();
        var userValidationResult = await userRegisterValidator.ValidateAsync(item);
        if (!userValidationResult.IsValid)
        {
            var errors = userValidationResult.Errors;
            var errorsString = string.Concat(errors);
            throw new Exception(errorsString);
        }
        if (_context.Users.Where(x => x.Login == user.Login).Count() > 1) throw new Exception("User with this login already exists");
        #endregion

        if (!HashProvider.VerifyHash(item.Password.Trim(), dbUser.OwnHashedPassword)) throw new Exception("Password not equal");
        if (!string.IsNullOrEmpty(item.PasswordChanged) && !string.IsNullOrEmpty(item.PasswordChangedConfirm))
        {
            if (item.PasswordChanged != item.PasswordChangedConfirm) throw new Exception("Changed password values ​​are not equal");
            user.OwnHashedPassword = HashProvider.ComputeHash(item.PasswordChangedConfirm.Trim());
        }

        dbUser = user;

        var result = _context.Users.Update(dbUser);

        var saved = false;
        while (!saved)
        {
            try
            {
                // Attempt to save changes to the database
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
                                proposedValues[property] = proposedValue; // Keep the proposed value
                            }

                            // TODO: decide which value should be written to database
                            // proposedValues[property] = <value to be saved>;
                        }

                        // Refresh original values to bypass next concurrency check
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





        if (item.IsStudent == true && user.UserStudent == null) user.UserStudent = new Student() { ApplicationUser = user, IsDeleted = true };
        if (item.IsTeacher == true && user.UserTeacher == null) user.UserTeacher = new Teacher() { ApplicationUser = user, IsDeleted = true };


        return await GetAsync(user.Id);

    }

    private async Task<ApplicationUser> DeleteAsync(Guid id)
    {
        var dbItem = await _context.Users.FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("User not found");
        dbItem.IsDeleted = true;
        var teacher = await _context.Teachers.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.ApplicationUser.Id == id);
        var student = await _context.Students.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.ApplicationUser.Id == id);
        if (teacher != null) teacher.IsDeleted = true;
        if (student != null) student.IsDeleted = true;

        await _userManager.UpdateAsync(dbItem);
        await _context.SaveChangesAsync();

        var roles = await _userManager.GetRolesAsync(dbItem);
        foreach (var role in roles)
        {
            await _userManager.RemoveFromRoleAsync(dbItem, role);
        }
        await _context.SaveChangesAsync();

        return dbItem;
    }
    public async Task<bool> IsInRole(ApplicationUser user, string role)
    {
        return await _userManager.IsInRoleAsync(user, role);
    }
    public async Task<ApplicationUser> SignInAsync(UserLoginDTO item)
    {
        var userRegisterValidator = new UserLoginValidator();
        var validationResult = await userRegisterValidator.ValidateAsync(item);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors;
            var errorsString = string.Concat(errors);
            throw new Exception(errorsString);
        }

        if (!_context.Users.Any(x => x.Login == item.Login)) throw new Exception("User with such login not exists");
        ApplicationUser? user = await _context.Users.Where(x => x.Login == item.Login).FirstOrDefaultAsync()
            ?? throw new Exception("User not found");

        var has1 = user.OwnHashedPassword;
        if (!HashProvider.VerifyHash(item.Password, has1)) throw new Exception("Incorrect password");

        await _signInManager.SignInAsync(user, true);

        return user;
    }
    public async Task<ApplicationUser> SignInAsync(ApplicationUser item)
    {
        return await SignInAsync(_mapper.Map<UserLoginDTO>(item));
    }
    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task CreateAdminAsync()
    {
        var resultAdmin = _context.Users.FirstOrDefault(x => x.Login == "AdminLogin");
        var resultAdminInfo = _context.BaseUserInfo.FirstOrDefault(x => x.FirstName == "AdminFirstName");
        if (resultAdmin != null && resultAdminInfo != null) return;

        if (resultAdminInfo == null)
        {
            resultAdminInfo = _context.BaseUserInfo.FirstOrDefault(x => x.FirstName == "AdminFirstName") ?? new BaseUserInfo()
            {

                FirstName = "AdminFirstName",
                MiddleName = "AdminMiddleName",
                Surname = "AdminSurname",
                Sex = "Male",
                //IsBase = true,
            };
            await _context.BaseUserInfo.AddAsync(resultAdminInfo);
            await _context.SaveChangesAsync();
        }
        if (resultAdmin == null)
        {
            resultAdmin = new ApplicationUser()
            {
                Login = "AdminLogin",
                UserName = "AdminLogin",
                //Password = "AdminPassword123",
                OwnHashedPassword = HashProvider.ComputeHash("AdminPassword123"),
                IsDeleted = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var res = await _context.Users.AddAsync(resultAdmin);
            await _context.SaveChangesAsync();
            resultAdmin = res.Entity;
            try
            {
                var re = await _userManager.AddToRoleAsync(resultAdmin, "Admin");
                await _context.SaveChangesAsync();
            }
            catch { }

        }
        var applicationUserBaseUserInfo = new ApplicationUserBaseUserInfo() { ApplicationUserId = resultAdmin.Id, BaseUserInfoId = resultAdminInfo.Id };
        await _context.ApplicationUserBaseUserInfo.AddAsync(applicationUserBaseUserInfo);

        await _context.SaveChangesAsync();

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

    protected async Task<ApplicationUser> SoftDeleteAsync(ApplicationUser item)
    {
        var roles = _context.UserRoles.Where(x => x.UserId == item.Id);
        _context.UserRoles.RemoveRange(roles);
        await _context.SaveChangesAsync();
        var teacher = item.UserTeacher;
        var student = item.UserStudent;

        if (teacher != null)
        {
            teacher.IsDeleted = true;
            //teacher.Groups
            //teacher.PossibleCources = new List<LessonTypeTeacher>();

            //var scheduleTeacher = teacher.WorkingDays;

            //await DeleteStudentAsync(teacher.Id);

            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();

        }
        if (student != null)
        {
            student.IsDeleted = true;

            /*
            var groupIdsWithStudent = _context.Groups;


            foreach (var group in _context.Groups.Include(x => x.GroupStudents))
            {
                var groupStudents = group.GroupStudents;
                if (groupStudents != null && groupStudents.FirstOrDefault(x => x.Student == student) != null)
                {
                    _context.GroupStudents.Remove(groupStudents.FirstOrDefault(x => x.Student == student));
                    //group.GroupStudents.Remove(student);
                }
            }

            student.Groups = new List<GroupStudent>();
            student.PossibleCources = new List<LessonTypeStudent>();

            
            student.Lessons = new List<LessonStudent>();
            */
            _context.Students.Update(student);
            await _context.SaveChangesAsync();

        }

        //var lessons = _context.Lessons.Include(x => x.Creator).
        //    Where(x => x.Creator.Id == item.Id);
        //foreach (var lesson in lessons)
        //{
        //    lesson.Creator = null;
        //    _context.Lessons.Update(lesson);
        //}

        /*
        if(item.EnglishLevel != null)
        {
            item.EnglishLevel = null;
            try
            {
                //var a = await _context.EnglishLevels.FirstOrDefaultAsync(x => x.Users.Contains(item));
                //a.Users.Remove(item);
                //_context.EnglishLevels.Update(a);
            }
        catch (Exception ex) { }
            
        }*/

        item.IsDeleted = true;
        _context.Users.Update(item);
        await _context.SaveChangesAsync();
        return item;
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
