using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using skills_hub.core.Repository.User.Interfaces;

namespace skills_hub.core.Repository.User;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public NotificationService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
    }



    public async Task<NotificationMessage> СreateToEditLesson(Lesson lastLessonValue, Lesson? newlessonValue = null, List<ApplicationUser>? usersToSend = null, int answer = 1)
    {
        //answer for: if Creator create request and his req was't apply send to him message about it
        NotificationMessage notification = new NotificationMessage();
        try
        {

            var group = await GetGroupAsync(lastLessonValue.GroupId ?? Guid.Empty);
            var notificationMessage = "";

            if (newlessonValue == null)
            {
                notificationMessage += "В группе " + group.Name + " занятие было удалено :  " //"In group " + group.Name + " lesson  was deleted by date:  "
                        + lastLessonValue.StartTime + " - " + lastLessonValue.EndTime;
            }
            else
            {
                if (answer > 0)
                {

                    notificationMessage += "В группе " + group.Name + " время занятия было изменено с " //"In group " + group.Name + " lesson  was changed from "
                        + lastLessonValue.StartTime + " - " + lastLessonValue.EndTime
                        + " на " + newlessonValue.StartTime + " - " + newlessonValue.EndTime;

                }
                else
                {
                    //notificationMessage += "В группе " + group.Name + $" время занятия {lastLessonValue.StartTime} не было изменено.";//"In group " + group.Name + " lesson  was`s changed.";
                    //usersToSend = new List<ApplicationUser>() { await _userService.GetCurrentUserAsync() };
                }
            }
            if (usersToSend == null) usersToSend = await GetUsersByGroup(group);


            var res = await Create(notificationMessage, usersToSend);
            return res;

        }
        catch { }

        return notification;

    }

    public async Task<NotificationMessage> СreateToUpdateCountLessonsInGroup(Group group, int previousCountLessons, int currentCountLessons, List<ApplicationUser>? usersToSend)
    {
        var message = "Count lessons was changed from " + previousCountLessons + " to " + currentCountLessons;
        if (usersToSend == null) usersToSend = await GetUsersByGroup(group);

        var res = await Create(message, usersToSend);
        return res;
    }




    public async Task CreateToCreateGroup(Group group, List<ApplicationUser> teachers = null, List<ApplicationUser> students = null, List<ApplicationUser> admins = null)
    {
        var adminsDb = await _userManager.GetUsersInRoleAsync("Admin");
        var message = $"Группа {group.Name} была создана";
        await Create(message, adminsDb.ToList());
        await CreateToAddUsersToGroup(group, teachers, students, admins);

    }
    public async Task CreateToRemoveGroup(Group group, List<ApplicationUser> teachers = null, List<ApplicationUser> students = null, List<ApplicationUser> admins = null)
    {
        var adminsDb = await _userManager.GetUsersInRoleAsync("Admin");
        var message = $"Группа {group.Name} была удалена";
        await Create(message, adminsDb.ToList());
        await CreateToRemoveUsersFromGroup(group, teachers, students, admins);

    }

    public async Task CreateToAddUsersToGroup(Group group, List<ApplicationUser> teachers = null, List<ApplicationUser> students = null, List<ApplicationUser> admins = null)
    {
        List<BaseUserInfo> users = new List<BaseUserInfo>();
        var messageToAdmin = "";


        if (teachers == null) teachers = _context.GroupTeachers.AsNoTracking().Where(x => x.GroupId == group.Id).Include(x => x.Teacher).ThenInclude(x => x.ApplicationUser).Select(x => x.Teacher).Select(x => x.ApplicationUser).ToList();
        if (students == null) students = _context.GroupStudents.AsNoTracking().Where(x => x.GroupId == group.Id).Include(x => x.Student).ThenInclude(x => x.ApplicationUser).Select(x => x.Student).Select(x => x.ApplicationUser).ToList();
        if (admins == null) admins = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Admin");

        foreach (var us in teachers)
        {
            var user = _context.ApplicationUserBaseUserInfo.Where(x => x.IsBase).Include(x => x.BaseUserInfo)?.FirstOrDefault(x => x.ApplicationUserId == us.Id);
            if (user == null) continue;
            users.Add(user.BaseUserInfo);
            var message = $"Учитель {user.BaseUserInfo.FullName} был добавлен в группу {group.Name} ";
            await Create(message, new List<ApplicationUser>() { new ApplicationUser() { Id = user.Id } });
        }
        foreach (var us in students)
        {
            var user = _context.ApplicationUserBaseUserInfo.Where(x => x.IsBase).Include(x => x.BaseUserInfo)?.FirstOrDefault(x => x.ApplicationUserId == us.Id);
            if (user == null) continue;
            users.Add(user.BaseUserInfo);
            var message = $"Ученик {user.BaseUserInfo.FullName} был добавлен в группу {group.Name} ";
            await Create(message, new List<ApplicationUser>() { new ApplicationUser() { Id = user.Id } });
        }
        if (string.IsNullOrEmpty(messageToAdmin)) return;
        foreach (var admin in admins)
        {
            var resMessage = $"Были добавлены в группу {group.Name} : " + messageToAdmin;
            await Create(resMessage, new List<ApplicationUser>() { new ApplicationUser() { Id = admin.Id } });

        }
    }
    public async Task CreateToRemoveUsersFromGroup(Group group, List<ApplicationUser> teachers = null, List<ApplicationUser> students = null, List<ApplicationUser> admins = null)
    {
        List<BaseUserInfo> users = new List<BaseUserInfo>();
        var messageToAdmin = "";

        if (teachers == null) teachers = _context.GroupTeachers.AsNoTracking().Where(x => x.GroupId == group.Id).Include(x => x.Teacher).ThenInclude(x => x.ApplicationUser).Select(x => x.Teacher).Select(x => x.ApplicationUser).ToList();
        if (students == null) students = _context.GroupStudents.AsNoTracking().Where(x => x.GroupId == group.Id).Include(x => x.Student).ThenInclude(x => x.ApplicationUser).Select(x => x.Student).Select(x => x.ApplicationUser).ToList();
        if (admins == null) admins = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Admin");

        /*
        if (usersToSend == null) usersToSend = await GetUsersByGroup(group);
        usersToSend = usersToSend.Except(admins).ToList();*/

        foreach (var us in teachers)
        {
            var user = _context.ApplicationUserBaseUserInfo.Where(x => x.IsBase).Include(x => x.BaseUserInfo)?.FirstOrDefault(x => x.ApplicationUserId == us.Id);
            if (user == null) continue;
            users.Add(user.BaseUserInfo);
            var message = $"Учитель {user.BaseUserInfo.FullName} был удален из группы {group.Name} ";
            messageToAdmin += $" {teachers.IndexOf(us) + 1}. {user.BaseUserInfo.FullName} ";
            await Create(message, new List<ApplicationUser>() { new ApplicationUser() { Id = user.Id } });
        }
        foreach (var us in students)
        {
            var user = _context.ApplicationUserBaseUserInfo.Where(x => x.IsBase).Include(x => x.BaseUserInfo)?.FirstOrDefault(x => x.ApplicationUserId == us.Id);
            if (user == null) continue;
            users.Add(user.BaseUserInfo);
            var message = $"Ученик {user.BaseUserInfo.FullName} был удален из группы {group.Name} ";
            messageToAdmin += $" {students.IndexOf(us) + 1}. {user.BaseUserInfo.FullName} ";
            await Create(message, new List<ApplicationUser>() { new ApplicationUser() { Id = user.Id } });
        }

        if (string.IsNullOrEmpty(messageToAdmin)) return;
        foreach (var admin in admins)
        {
            var resMessage = $"Были удалены из группы {group.Name} : " + messageToAdmin;
            await Create(resMessage, new List<ApplicationUser>() { new ApplicationUser() { Id = admin.Id } });

        }

    }


    private async Task<NotificationMessage> Create(NotificationMessage notification, List<ApplicationUser> usersToSend)
    {
        var notificationUsers = usersToSend.Select(user => new domain.Models.ManyToMany.NotificationUser()
        {
            UserId = user.Id,
            NotificationMessageId = notification.Id
        }).ToList();

        await _context.NotificationUsers.AddRangeAsync(notificationUsers);
        await _context.SaveChangesAsync();
        /*
        foreach (var user in usersToSend)
        {
            var u = new ApplicationUser() { Id = user.Id };
            var r = new NotificationUser() { User = u, NotificationMessage = notification };
           // _context.Entry(r.User).State = EntityState.Unchanged;
            //_context.Entry(r.NotificationMessage).State = EntityState.Unchanged;
            await _context.NotificationUsers.AddAsync(r);

        }
        await _context.SaveChangesAsync();*/
        return notification;
    }

    public async Task<NotificationMessage> Create(string message, List<ApplicationUser> usersToSend)
    {
        var admins = await _userManager.GetUsersInRoleAsync("Admin");
        if (usersToSend == null) usersToSend = admins.ToList();

        var notification = new NotificationMessage() { Message = message };



        try
        {
            await _context.NotificationMessages.AddAsync(notification);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                if (entry.Entity is NotificationMessage)
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




        var res = await Create(notification, usersToSend);
        return res;
    }

    public async Task<List<ApplicationUser>> GetUsersByGroup(Group group)
    {

        var usersToSend = new List<ApplicationUser>();

        var students = _context.GroupStudents.AsNoTracking().Where(x => x.GroupId == group.Id).Include(x => x.Student).ThenInclude(x => x.ApplicationUser).Select(x => x.Student).Select(x => x.ApplicationUser).ToList();
        var teachers = _context.GroupTeachers.AsNoTracking().Where(x => x.GroupId == group.Id).Include(x => x.Teacher).ThenInclude(x => x.ApplicationUser).Select(x => x.Teacher).Select(x => x.ApplicationUser).ToList();
        var admins = await _userManager.GetUsersInRoleAsync("Admin");

        usersToSend.AddRange(students);
        usersToSend.AddRange(teachers);
        usersToSend.AddRange(admins);
        return usersToSend;

    }

    public async Task<Group> GetGroupAsync(Guid id)
    {
        var groups = await _context.Groups
            .Include(x => x.Lessons)
            .Include(x => x.Lessons).ThenInclude(x => x.Teacher).ThenInclude(x => x.Teacher).ThenInclude(x => x.ApplicationUser)
            .Include(x => x.Lessons).ThenInclude(x => x.ArrivedStudents).ThenInclude(x => x.Student).ThenInclude(x => x.ApplicationUser)
            .Include(x => x.Lessons).Include(x => x.LessonType)
            //.Include(x => x.GroupStudents).ThenInclude(x => x.Group)
            .Include(x => x.GroupStudents).ThenInclude(x => x.Student).ThenInclude(x => x.ApplicationUser)
            //.Include(x => x.GroupStudents).ThenInclude(x => x.Group).ThenInclude(x => x.Lessons)
            .Include(x => x.DaySchedules)
            .Include(x => x.GroupTeachers).ThenInclude(x => x.Teacher).ThenInclude(x => x.ApplicationUser)
            .Include(x => x.LessonType).ToListAsync();
        var item = groups.Find(x => x.Id == id);
        return item;
    }
}
