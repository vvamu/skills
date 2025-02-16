using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using skills_hub.core.Repository.User;
using skills_hub.core.Repository.User.Interfaces;
using skills_hub.core.Validators;
using skills_hub.domain.Models.ManyToMany;


namespace skills_hub.core.repository;

public class GroupService : AbstractLogModelService<Group>, IGroupService
{
    private readonly ILessonService _lessonService;
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    private readonly IIncludableQueryable<Group, PaymentCategory>? _fullInclude;

    public GroupService(ApplicationDbContext context)//, INotificationService notificationService), ILessonService lessonService, IUserService userService, INotificationService notificationService)
    {
        _context = context;
        _validator = new GroupValidator();
        _contextModel = _context.Groups;

        _fullInclude = _context.Groups
            .Include(x => x.Lessons).ThenInclude(x => x.Teacher).ThenInclude(x => x.Teacher).ThenInclude(x => x.ApplicationUser)
            .Include(x => x.Lessons).ThenInclude(x => x.ArrivedStudents).ThenInclude(x => x.Student).ThenInclude(x => x.ApplicationUser)
            .Include(x => x.GroupStudents).ThenInclude(x => x.Student).ThenInclude(x => x.ApplicationUser).ThenInclude(x => x.ConnectedUsersInfo).ThenInclude(x => x.BaseUserInfo)
            .Include(x => x.GroupTeachers).ThenInclude(x => x.Teacher).ThenInclude(x => x.ApplicationUser).ThenInclude(x => x.ConnectedUsersInfo).ThenInclude(x => x.BaseUserInfo)
            .Include(x => x.DaySchedules)
            .Include(x => x.LessonType).ThenInclude(x => x.AgeType)
            .Include(x => x.LessonType).ThenInclude(x => x.Course)
            .Include(x => x.LessonType).ThenInclude(x => x.LessonTypePaymentCategory).ThenInclude(x => x.PaymentCategory)
            .Include(x => x.LessonType).ThenInclude(x => x.GroupType)
            .Include(x => x.LessonType).ThenInclude(x => x.Location)
            .Include(x => x.LessonType).ThenInclude(x => x.AgeType)
            .Include(x => x.PaymentCategory);
        //_notificationService = notificationService;

    }
    
    #region Get
    public async Task<Group> GetItemsListAsync(Guid id) => await _context.Groups
            .Include(x => x.Lessons)
            .Include(x => x.GroupStudents)//.ThenInclude(x => x.Student).ThenInclude(x => x.ApplicationUser).ThenInclude(x => x.ConnectedUsersInfo).ThenInclude(x => x.BaseUserInfo)
            .Include(x => x.GroupTeachers).ThenInclude(x => x.Teacher).ThenInclude(x => x.ApplicationUser).ThenInclude(x => x.ConnectedUsersInfo).ThenInclude(x => x.BaseUserInfo)
            .Include(x => x.DaySchedules)
            .Include(x => x.LessonType).ThenInclude(x => x.AgeType)
            .Include(x => x.LessonType).ThenInclude(x => x.Course)
            //.Include(x => x.LessonType).ThenInclude(x => x.LessonTypePaymentCategory).ThenInclude(x => x.PaymentCategory)
            .Include(x => x.LessonType).ThenInclude(x => x.GroupType)
            .Include(x => x.LessonType).ThenInclude(x => x.Location)
            .Include(x => x.LessonType).ThenInclude(x => x.AgeType)
            .Include(x => x.PaymentCategory).FirstOrDefaultAsync(x => x.Id == id);



    #endregion

    #region Create
    public async Task<Group> CreateAsync(Group item, Guid[] studentId, string[] dayName, TimeSpan[] startTime)
    {

        if (item.IsLateDateStart) item.DateStart = DateTime.MinValue;
        else item.IsCanAddLessons = true;
        var lessonType = await _context.LessonTypes.FindAsync(item.LessonTypeId);
        item.LessonsCount = lessonType.PreparedLessonsCount;
        List<Lesson> lessons = new List<Lesson>();

        await Validate(item,studentId,dayName,startTime);
        var groupDb = await base.CreateAsync(item);
        var message = $"Группа {groupDb.Name} была создана";
        //await _notificationService.Create(message);
        await UpdateGroupTeachers(new Group(), groupDb, new List<Guid>() { item.TeacherId });
        await UpdateGroupStudents(new Group(),groupDb, studentId.ToList());
        var schedule = await CreateScheduleDaysToGroup(groupDb, dayName, startTime);
        if (item.IsVerified && !item.IsLateDateStart && item.IsCreateLessonsAlready)
        {
            lessons = await CreateLessonsBySchedule(schedule, item.DateStart, item.LessonsCount, item, true);
            if (item.IsPermanentStaffGroup)
            {
                foreach (var lesson in lessons)
                {
                    try
                    {
                        //await _lessonService.UpdateLessonStudents(lesson, studentId.ToList());
                    }
                    catch (Exception ex) { throw new Exception("The group was created, but messages were not sent to users."); }
                }
            }
        }

        if (!item.IsVerified)
        {
            var user = await _userService.GetCurrentUserAsync();
            var message2 = "Teacher " + user.FirstName + " " + user.MiddleName + " " + user.Surname + " send request to create new group '" + item.Name + "'. Check it.";
            var notification = new NotificationMessage() { IsRequest = true, Message = message2 };
            /////////////////////////////
        }
        await _context.SaveChangesAsync();



        return groupDb;
    }


    #endregion
    

    public async Task<Group> UpdateAsync(Group item, Guid[] studentId, string[] dayName, TimeSpan[] startTime)
    {
        var oldGroup = _context.Groups.Include(x => x.GroupTeachers).Include(x => x.GroupStudents).FirstOrDefault(x => x.Id == item.Id);
        if (item.IsLateDateStart) item.DateStart = DateTime.MinValue;
        else item.IsCanAddLessons = true;
        List<Lesson> lessons = _context.Lessons.Where(x => x.GroupId != null && x.GroupId == item.Id).ToList();
        Group group = new Group();
        int countLessonsToCreate = 0;

        var lessonType = await _context.LessonTypes.FindAsync(item.LessonTypeId);
        if (lessonType != null) countLessonsToCreate = lessonType.PreparedLessonsCount - _context.Lessons.Where(x => x.GroupId == item.Id).Where(x => x.ParentId == null).Count();
        //item.LessonsCount = lessonType.Result.PreparedLessonsCount;
        if (lessons.Count() != 0)
        {
            //Не трогать расписание, Payment, LessonType

            /*
                 public DateTime DateStart { get; set; }
                public bool IsLateDateStart { get; set; }
                public string Name { get; set; }
                public List<GroupTeacher>? GroupTeachers { get; set; }
                public List<GroupStudent>? GroupStudents { get; set; }
             */
        }

        await Validate(item, studentId, dayName, startTime);
        group = await base.UpdateAsync(item);
        await UpdateGroupTeachers(oldGroup, group, new List<Guid>() { item.TeacherId });
        await UpdateGroupStudents(oldGroup, group, studentId.ToList());
        //await UpdateGroupStudents(oldGroup, group, studentId.ToList());
        var schedule = await CreateScheduleDaysToGroup(group, dayName, startTime);
        if (item.IsVerified && !item.IsLateDateStart && item.IsCreateLessonsAlready)
        {
            await CreateLessonsBySchedule(schedule, group.DateStart, countLessonsToCreate, item, true);
        }
        await _lessonService.UpdateLessonsUsersForUnCompletedLessonsByGroupAsync(group.Id, group.TeacherId);
        /*
                }
                catch(Exception ex )
                {
                    try
                    {

                        //await Validate(oldGroup, item);

                        item.Id = Guid.Empty;
                        item.DateCreated = DateTime.Now;
                        var res = await _contextModel.AddAsync(item);
                        var resCreated = res.Entity;


                        var children = GetAllParents(oldGroup.Id).OrderByDescending(x => x.DateCreated).ToList();
                        var parents = GetAllChildren(oldGroup.Id).OrderByDescending(x => x.DateCreated).ToList();

                        if (oldGroup.Id != Guid.Empty) parents.Insert(0, oldGroup);
                        if (children.Count() != 0)
                        {
                            res.Entity.DateRegistration = parents.LastOrDefault()?.DateCreated;
                        }
                        else
                        {
                            res.Entity.DateRegistration = res.Entity.DateCreated;
                        }
                        await _context.SaveChangesAsync();

                        ////

                        oldGroup.ParentId = resCreated.Id;
                        _context.Groups.Update(oldGroup);
                        await _context.SaveChangesAsync();
                        ////////


    }
            catch (Exception ex2){ }*/

        return item;
    }

    public async Task<Group?> DeleteAsync(Guid id, bool isHardDelete)
    {
        Group? result = null;
        if (isHardDelete)
        {
            result = await HardDeleteAsync(id);
        }
        else
        {
            var groupDb = await _context.Groups.FindAsync(id);
            groupDb.IsDeleted = true;
            var res = _context.Groups.Update(groupDb);
            await _context.SaveChangesAsync();
            result = res.Entity;
        }
        return result;
    }
    public async Task<Group> HardDeleteAsync(Guid id)
    {
        //lessons <- requests <- lessonStudents <- lessonTeachers;

        var group = await _context.Groups.FindAsync(id);
        await _notificationService.CreateToRemoveGroup(group);


        var students = await _context.GroupStudents.AsNoTracking().Where(x => x.GroupId == group.Id).ToListAsync();
        var teachers = await _context.GroupTeachers.AsNoTracking().Where(x => x.GroupId == group.Id).ToListAsync();
        var lessons = await _context.Lessons.AsNoTracking().Where(x => x.GroupId == group.Id).ToListAsync();
        var schedules = await _context.GroupWorkingDays.AsNoTracking().Where(x => x.GroupId == group.Id).ToListAsync();

        foreach (var i in lessons)
        {
            await _lessonService.Delete(i, true, false);

        }
        _context.GroupStudents.RemoveRange(students);
        await _context.SaveChangesAsync();
        _context.GroupTeachers.RemoveRange(teachers);
        await _context.SaveChangesAsync();
        _context.GroupWorkingDays.RemoveRange(schedules);
        await _context.SaveChangesAsync();

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();

        return null;
    }


    #region Helpers

    public async Task Validate(Group item, Guid[] studentId, string[] dayName, TimeSpan[] startTime)
    {
        if (item == null) throw new Exception("Not correct data for group");
        if (item.IsLateDateStart) item.DateStart = DateTime.MinValue;
        var lessonType = _context.LessonTypes.Where(x => x.ParentId == null).Include(x => x.GroupType).Include(x => x.AgeType).FirstOrDefault(x => x.Id == item.LessonTypeId);
        if (lessonType == null) throw new Exception("Lesson type not found");
        if (!lessonType.IsActive || lessonType.IsDeleted) throw new Exception("Lesson type is not active");
        var groupType = await _context.GroupTypes.FindAsync(lessonType.GroupTypeId);
        if (groupType == null) throw new Exception("Group type by lesson type not found");
        var teacher = await _context.Teachers.FirstOrDefaultAsync(x => x.Id == item.TeacherId); //.Include(x=>x.PossibleCources).
        var students = _context.Students.Where(s => studentId.Contains(s.Id)).Where(x => !x.IsDeleted).Include(x => x.ApplicationUser).ThenInclude(x => x.ConnectedUsersInfo).ThenInclude(x => x.BaseUserInfo).ToList(); //Include(x => x.PossibleCources).
        if (!item.IsLateDateStart)
        {
            if (teacher == null || teacher.IsDeleted) throw new Exception("Teacher now is not active");
            if (students == null || students.Any(x => x.IsDeleted)) throw new Exception("Students not selected or one of them now is not active");



        }
        if (studentId.Contains(item.TeacherId)) throw new Exception("The teacher cannot teach in the same group");
        //if (teacher?.PossibleCources != null && teacher.CurrentPossibleCourses != null && !teacher.CurrentPossibleCourses.Select(x => x.LessonTypeId).Contains(item.LessonTypeId)) throw new Exception($"The teacher is not able to teach the chosen class category");
        //foreach (var student in students)
        //{
        //    if (student.PossibleCources != null && student.CurrentPossibleCourses != null && !student.CurrentPossibleCourses.Select(x => x.LessonTypeId).Contains(item.LessonTypeId)) throw new Exception($"Student {student?.ApplicationUser?.UserInfo?.FullName} does not want to learn the chosen type of lesson");
        //}



        //if ((!item.IsUnlimitedLessonsCount && lessonType.PreparedLessonsCount < 0) || !lessonType.IsUnlimitedLessonsCount) throw new Exception("Invalid lessons count value");

        //await CheckCorrectWorkingDays(dayName, startTime);

        var isCorrectCountStudents = lessonType.CheckGroupType && (studentId.Count() <= groupType.MaximumStudents) && (studentId.Count() >= groupType.MinimumStudents);
        if (!isCorrectCountStudents && !item.IsLateDateStart) throw new Exception("Некорректное количество учеников в группе. Необходимо: " + groupType.DisplayName);

        if (lessonType.CheckAgeType)
        {
            var ageType = await _context.AgeTypes.FindAsync(lessonType.AgeTypeId);
            var usersInfo = await _context.Students.Include(x => x.ApplicationUser).ThenInclude(x => x.ConnectedUsersInfo).ThenInclude(x => x.BaseUserInfo).ToListAsync();
            foreach (var studId in studentId)
            {

                var userInfo = usersInfo.FirstOrDefault(x => x.Id == studId).ApplicationUser.UserInfo;
                if (userInfo.Age > ageType.MinimumAge || userInfo.Age < ageType.MaximumAge) throw new Exception($"Age of student '{userInfo.FullName}' - {userInfo.Age}. Required age to group: {ageType.DisplayName}");

            }
        }

        if (lessonType.CheckCountScheduleDays)
        {
            if (lessonType.FrequencyName == "week" && lessonType.FrequencyValue != dayName.Length && lessonType.FrequencyValue != startTime.Length) throw new Exception($"Количество занятий в группе в неделю должно быть - {lessonType.FrequencyValue} ");

        }


    }
    public async Task<List<Lesson>> CreateLessonsBySchedule(List<GroupWorkingDay> schedules, DateTime startDate, int countLessons, Group group, bool isVerified)
    {
        var lessons = new List<Lesson>();
        var groupLessons = _context.Lessons.Include(x => x.Group).Where(x => x.GroupId == group.Id);
        try
        {
            var date = startDate;//hz
            if (countLessons < 0) countLessons = 100;
            for (int lesCount = 0, scCount = 0; lesCount < countLessons; lesCount++, scCount++)
            {
                if (scCount == schedules.Count)
                {
                    scCount = 0;
                    date = date.AddDays(7);
                }
                var addingDays = LessonMath.Mod((int)date.DayOfWeek, (int)schedules[scCount].DayName);
                var virtualDate = date.AddDays(addingDays);
                var lesson = new Lesson()
                {
                    StartTime = virtualDate + schedules[scCount].WorkingStartTime ?? DateTime.Now,
                    EndTime = virtualDate + schedules[scCount].WorkingEndTime ?? DateTime.Now,
                    GroupId = group.Id, ///
                };

                lessons.Add(lesson);

                if (groupLessons != null && !groupLessons.Any(x => x.StartTime == lesson.StartTime && x.EndTime == lesson.EndTime))
                {
                    _context.Lessons.Add(lesson);

                }

            }
            /*
            group.Lessons = lessons;
            try
            {
                //_context.Entry(group.Teacher).State = EntityState.Unchanged;
                //_context.Entry(group.Teacher.ApplicationUser).State = EntityState.Unchanged;
                await _context.SaveChangesAsync();
                _context.Groups.Update(group);
                var entry = _context.Entry<Group>(group);
                _context.Entry(group).State = EntityState.Unchanged;

            }
            catch (Exception ex) { }
            await _context.SaveChangesAsync();
            */


            //await AddStudentsToLessons(group, group.GroupStudents.Select(x => x.Id).ToList(), lessons);

        }



        catch (Exception ex) { }

        return lessons;


    }
    public async Task<Group> UpdateGroupStudents(Group oldGroup, Group newGroup, List<Guid> studentsId)
    {
        List<Guid> dbStudentsId = new List<Guid>();
        if (oldGroup.Id != Guid.Empty && oldGroup.GroupStudents != null) dbStudentsId = oldGroup.GroupStudents.Where(x => !x.IsDeleted).Select(x => x.StudentId).ToList();
        List<Guid> toCreate = studentsId.Except(dbStudentsId).ToList();
        List<Guid> toUpdate = studentsId.Intersect(dbStudentsId).ToList();
        List<Guid> toDelete = dbStudentsId.Except(studentsId).ToList();

        foreach (var studentId in toCreate)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null) break;

            var grSt = new GroupStudent() { GroupId = newGroup.Id, StudentId = studentId };
            await _context.GroupStudents.AddAsync(grSt);
            await _context.SaveChangesAsync();

            /*
            #region Create notification when user was added to group
            var usersToSend = new List<NotificationUser>() { new NotificationUser() { UserId = student.ApplicationUserId } };
            var message = " You was added to group " + newGroup.Name;
            var notification = new NotificationMessage() { Message = message, Users = usersToSend };

            usersToSend.ForEach(x => x.NotificationMessage = notification);
            await _context.NotificationUsers.AddRangeAsync(usersToSend.AsEnumerable());
            await _context.NotificationMessages.AddAsync(notification);
            await _context.SaveChangesAsync();
            #endregion*/
        }
        foreach (var studentId in toUpdate)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null) break;

            var grSt = new GroupStudent() { GroupId = newGroup.Id, StudentId = studentId };
            await _context.GroupStudents.AddAsync(grSt);
            await _context.SaveChangesAsync();
        }
        foreach (var studentId in toDelete)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null) break;
            var grSt = new GroupStudent() { GroupId = newGroup.Id, StudentId = studentId, IsDeleted = true };
            await _context.GroupStudents.AddAsync(grSt);
            await _context.SaveChangesAsync();
            /*
            #region Create notification to remove from group
            try
            {
                var stud = await _context.Students.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.Id == student.Id);
                var usersToSend = new List<NotificationUser>() { new NotificationUser() { UserId = student.ApplicationUserId } };
                var message = " You was removed from group " + oldGroup.Name;
                var notification = new NotificationMessage() { Message = message, Users = usersToSend };

                usersToSend.ForEach(x => x.NotificationMessage = notification);
                await _context.NotificationUsers.AddRangeAsync(usersToSend.AsEnumerable());
                await _context.NotificationMessages.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
            catch { }
            #endregion*/
        }
        //await _notificationService.CreateToChangeStudentsByGroup(oldGroup, toCreate, toDelete);
        return newGroup;

    }
    public async Task<Group> UpdateGroupTeachers(Group oldGroup, Group newGroup, List<Guid> teachersId)
    {
        List<Guid> dbStudentsId = new List<Guid>();
        if (oldGroup.Id != Guid.Empty && oldGroup.GroupTeachers != null) dbStudentsId = oldGroup.GroupTeachers.Where(x => !x.IsDeleted).Select(x => x.TeacherId).ToList();
        List<Guid> toCreate = teachersId.Except(dbStudentsId).ToList();
        List<Guid> toUpdate = teachersId.Intersect(dbStudentsId).ToList();
        List<Guid> toDelete = dbStudentsId.Except(teachersId).ToList();

        foreach (var teacherId in toCreate)
        {
            var student = await _context.Teachers.FindAsync(teacherId);
            if (student == null) break;

            var grSt = new GroupTeacher() { GroupId = newGroup.Id, TeacherId = teacherId };
            //var teachers = await _context.GroupTeachers.Where(x => x.TeacherId == teacherId && x.GroupId == oldGroup.Id);

            //await _groupTeacherService.CreateAsync(grSt);

            await _context.GroupTeachers.AddAsync(grSt);
            await _context.SaveChangesAsync();
            #region Create notification when user was added to group
            //await _notificationService.Create("You was added to group " + newGroup.Name + " like teacher", new List<ApplicationUser>() { new ApplicationUser() { Id = student.ApplicationUserId } });

            await _context.SaveChangesAsync();
            #endregion
        }
        foreach (var teacherId in toUpdate)
        {
            var teacher = await _context.GroupTeachers.FirstOrDefaultAsync(x => x.TeacherId == teacherId && x.GroupId == oldGroup.Id);
            if (teacher == null) break;

            //var grSt = new GroupTeacher() { GroupId = newGroup.Id, TeacherId = teacherId }; // Id = teacher.Id,
            //await _groupTeacherService.UpdateAsync(grSt);
            //await _context.GroupTeachers.AddAsync(grSt);
            //await _context.SaveChangesAsync();
        }
        foreach (var teacherId in toDelete)
        {
            var teacher = await _context.GroupTeachers.FirstOrDefaultAsync(x => x.TeacherId == teacherId && x.GroupId == oldGroup.Id);
            if (teacher == null) break;

            var grSt = new GroupTeacher() { GroupId = newGroup.Id, TeacherId = teacherId, IsDeleted = true }; // Id = teacher.Id,
            await _context.GroupTeachers.AddAsync(grSt);
            await _context.SaveChangesAsync();
            //temporary
            /*
            var toDel = await _context.GroupStudents.FirstOrDefaultAsync(x=>x.GroupId == group.Id &&  x.StudentId == studentId);
            _context.GroupStudents.Remove(toDel);
            _context.SaveChanges();*/

            #region Create notification to remove from group
            try
            {
                //await _notificationService.Create("You was removed from group" + oldGroup.Name + " like teacher", new List<ApplicationUser>() { new ApplicationUser() { Id = student.ApplicationUserId } });

                await _context.SaveChangesAsync();
            }
            catch { }
            #endregion
        }
        //await _notificationService.CreateToChangeTeachersByGroup(oldGroup, toCreate, toDelete);

        return oldGroup;
    }
    private async Task<bool> CheckCorrectWorkingDays(string[] dayName, TimeSpan[] startTime)
    {
        if (startTime.Count() != dayName.Count()) throw new Exception("Time is not defined to schedule");
        if (startTime.Count() > 6) throw new Exception("To many schedule days");
        var dayTimeDict = new Dictionary<string, TimeSpan>();
        for (int i = 0; i < dayName.Length; i++)
        {
            if (dayTimeDict.ContainsKey(dayName[i])) continue;
            dayTimeDict.Add(dayName[i], startTime[i]);
        }
        return true;
    }
    public async Task<List<GroupWorkingDay>> CreateScheduleDaysToGroup(Group item, string[] dayName, TimeSpan[] startTime)
    {
        Dictionary<string, string> russianToEnglishDays = new Dictionary<string, string>
        {
            { "Понедельник", "Monday" },
            { "Вторник", "Tuesday" },
            { "Среда", "Wednesday" },
            { "Четверг", "Thursday" },
            { "Пятница", "Friday" },
            { "Суббота", "Saturday" },
            { "Воскресенье", "Sunday" }
        };

        List<GroupWorkingDay> schedules = new List<GroupWorkingDay>();
        LessonType lessonType = await _context.LessonTypes.FindAsync(item.LessonTypeId);
        var duration = lessonType.LessonTimeInMinutes;
        var dbSchedule = await _context.GroupWorkingDays.Where(x => x.GroupId == item.Id).ToListAsync();



        //Create workingDat = schedule with dayName, startTime and endTime
        for (int i = 0; i < dayName.Count(); i++)
        {
            string englishDay = russianToEnglishDays[dayName[i]];
            var scheduleDay = new GroupWorkingDay()
            {
                DayName = Enum.Parse<DayOfWeek>(englishDay),
                WorkingStartTime = startTime[i],
                WorkingEndTime = startTime[i] + TimeSpan.FromMinutes(duration),
                GroupId = item.Id
            };

            var validations = new WorkingDayValidator();
            var validationResult = await validations.ValidateAsync(scheduleDay);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors;
                var errorsString = string.Concat(errors);
                throw new Exception(errorsString);
            }
            var foundDay = dbSchedule.FirstOrDefault(x => x.Equals(scheduleDay));
            if (foundDay != null) continue;

            schedules.Add(scheduleDay);
        }
        await _context.GroupWorkingDays.AddRangeAsync(schedules);
        //item.DaySchedules = schedules;

        //_context.Groups.Update(item);

        await _context.SaveChangesAsync();


        try
        {
            /*
            //var group = await _context.Groups.Include(x => x.DaySchedules).FirstOrDefaultAsync(x => x.Id == item.Id);
            if (item != null && item.DaySchedules != null)
            {

                var scheduleDaysInGroup = item.DaySchedules;

                //_context.GroupWorkingDays.RemoveRange(scheduleDaysInGroup);
                //item.DaySchedules = new List<GroupWorkingDay>();
                //_context.Groups.Update(item);
               // await _context.SaveChangesAsync();
                //var item22 = new List<GroupWorkingDay>();

                //item = group;
            }
            */





        }
        catch (Exception ex) { }

        return schedules;
    }
    #endregion






}
