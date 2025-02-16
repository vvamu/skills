using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using skills_hub.core.DTO;
using skills_hub.core.Validators;
using skills_hub.domain.Models.ManyToMany;

namespace skills_hub.core.Repository.User;

public class UserService : UserRepository, IUserService
{
    public UserService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
        ApplicationDbContext context, IMapper mapper, IAbstractLogModel<BaseUserInfo> baseUserInfoService) : base(signInManager, userManager, context, mapper, baseUserInfoService)
    { CreateAdminAsync(); }

    public async Task<ApplicationUser> GetAsync(Guid id)
    {
        var user = await base.GetAsync(id);

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
 
        var result = await base.CreateAsync(item);

        #region AddToRoleAsync(result, "Student");
        if (item.IsStudent == true)
        {
            var st = new Student() { ApplicationUserId = result.Id, IsDeleted = false }; await _context.Students.AddAsync(st); await _context.SaveChangesAsync();

            try
            {
                await _userManager.AddToRoleAsync(result, "Student");
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
                    else throw new NotSupportedException("Concurrency conflict occurred for entity type: " + entry.Entity.GetType().Name);
                }

                // Повторно пытаемся сохранить изменения
                _context.SaveChanges();
            }
        }
        #endregion
        #region AddToRoleAsync(result, "Teacher");
        if (item.IsTeacher == true)
        {
            var tch = new Teacher() { ApplicationUserId = result.Id, IsDeleted = false }; await _context.Teachers.AddAsync(tch); await _context.SaveChangesAsync();

            try
            {
                await _userManager.AddToRoleAsync(result, "Teacher");
                await _context.SaveChangesAsync();
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
                            entry.OriginalValues.SetValues(proposedValues);
                        }
                    }
                    else throw new NotSupportedException("Concurrency conflict occurred for entity type: " + entry.Entity.GetType().Name);
                }
                _context.SaveChanges();
            }
        }
        #endregion
        return await GetAsync(result.Id);
    }

    public async Task<ApplicationUser> UpdateAsync(UserCreateDTO item)
    {
        var user = await base.UpdateAsync(item);

        ////add roles if not exists 
        //if (item.IsStudent == true && user.UserStudent == null) user.UserStudent = new Student() { ApplicationUser = user, IsDeleted = true };
        //if (item.IsTeacher == true && user.UserTeacher == null) user.UserTeacher = new Teacher() { ApplicationUser = user, IsDeleted = true };


        return await GetAsync(user.Id);

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

        if (!_context.Users.Any(x => x.UserName == item.UserName)) throw new Exception("User with such login not exists");
        ApplicationUser? user = await _context.Users.Where(x => x.UserName == item.UserName).FirstOrDefaultAsync()
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
        var resultAdmin = _context.Users.FirstOrDefault(x => x.UserName == "AdminLogin");
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

    protected override async Task<ApplicationUser> SoftDeleteAsync(ApplicationUser item)
    {
        var dbItem = await base.SoftDeleteAsync(item);
        var roles = _context.UserRoles.Where(x => x.UserId == item.Id).Select(x => x.RoleId);
        var stringRoles = _context.Roles.Where(x => roles.Contains(x.Id)).Select(x => x.Name);
        foreach (var role in stringRoles)
        {
            await _userManager.RemoveFromRoleAsync(dbItem, role);
        }
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


        await _context.SaveChangesAsync();

        item.IsDeleted = true;
        _context.Users.Update(item);
        await _context.SaveChangesAsync();
        return item;
    }
}
