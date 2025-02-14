using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using skills_hub.core.Repository.User.Interfaces;
using skills_hub.core.Repository.User;
using skills_hub.core.repository;
using skills_hub.core.Validators;
using skills_hub.domain.Models.ManyToMany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skills_hub.core.Repository;

public class LessonService : AbstractLogModelService<Lesson>, ILessonService
{
    //private readonly IRequestService _requestService;
    private readonly IUserService _userService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;
    private readonly IQueryable<Lesson>? _fullInclude;


    public LessonService(ApplicationDbContext context, IUserService userService, UserManager<ApplicationUser> userManager)//, INotificationService notificationService)
    {
        _context = context;
        _contextModel = _context.Lessons;
        _validator = new LessonValidator();

        // _requestService = requestService;
        _userService = userService;
        _userManager = userManager;
        //_notificationService = notificationService;
        _fullInclude = _context.Lessons
            .Include(x => x.Group).ThenInclude(x => x.LessonType).ThenInclude(x => x.AgeType)
            .Include(x => x.Group).ThenInclude(x => x.LessonType).ThenInclude(x => x.Course)
            .Include(x => x.Group).ThenInclude(x => x.LessonType).ThenInclude(x => x.LessonTypePaymentCategory).ThenInclude(x => x.PaymentCategory)
            .Include(x => x.Group).ThenInclude(x => x.LessonType).ThenInclude(x => x.GroupType)
            .Include(x => x.Group).ThenInclude(x => x.LessonType).ThenInclude(x => x.Location)
            .Include(x => x.Group).ThenInclude(x => x.LessonType).ThenInclude(x => x.AgeType)
            .Include(x => x.Group).ThenInclude(x => x.PaymentCategory)

            .Include(x => x.Group).ThenInclude(x => x.GroupTeachers).ThenInclude(x => x.Teacher).ThenInclude(x => x.ApplicationUser).ThenInclude(x => x.ConnectedUsersInfo).ThenInclude(x => x.BaseUserInfo)
            .Include(x => x.Group).ThenInclude(x => x.GroupStudents).ThenInclude(x => x.Student).ThenInclude(x => x.ApplicationUser).ThenInclude(x => x.ConnectedUsersInfo).ThenInclude(x => x.BaseUserInfo)
            .Include(x => x.Teacher).ThenInclude(x => x.Teacher).ThenInclude(x => x.ApplicationUser).ThenInclude(x => x.ConnectedUsersInfo).ThenInclude(x => x.BaseUserInfo)
            .Include(x => x.ArrivedStudents).ThenInclude(x => x.Student).ThenInclude(x => x.ApplicationUser).ThenInclude(x => x.ConnectedUsersInfo).ThenInclude(x => x.BaseUserInfo)
            .AsNoTracking();
    }

    #region Get

    public IQueryable<Lesson> GetAll()
    {
        var items = _fullInclude
            .OrderBy(x => x.StartTime).AsQueryable();
        return items;
    }

    public IQueryable<Lesson> GetGroupLessonsList()
    {
        var items = _context.Lessons

            .Include(x => x.Group).ThenInclude(x => x.GroupStudents)
            .Include(x => x.Group).ThenInclude(x => x.LessonType).ThenInclude(x => x.GroupType)
            .Include(x => x.Group).ThenInclude(x => x.PaymentCategory)
            //.Include(x => x.Group).ThenInclude(x => x.GroupTeachers).ThenInclude(x => x.Teacher).ThenInclude(x => x.ApplicationUser).ThenInclude(x => x.ConnectedUsersInfo).ThenInclude(x => x.BaseUserInfo)
            //.Include(x => x.Group).ThenInclude(x => x.GroupStudents).ThenInclude(x => x.Student).ThenInclude(x => x.ApplicationUser).ThenInclude(x => x.ConnectedUsersInfo).ThenInclude(x => x.BaseUserInfo)
            .Include(x => x.Teacher).ThenInclude(x => x.Teacher).ThenInclude(x => x.ApplicationUser).ThenInclude(x => x.ConnectedUsersInfo).ThenInclude(x => x.BaseUserInfo)
            .Include(x => x.ArrivedStudents).ThenInclude(x => x.Student).ThenInclude(x => x.ApplicationUser).ThenInclude(x => x.ConnectedUsersInfo).ThenInclude(x => x.BaseUserInfo)
            .AsNoTracking()
            .OrderBy(x => x.StartTime).AsQueryable();
        return items;
    }

    protected override async Task<Lesson> GetAsync(Guid? itemId, bool withParents = false)
    {
        var id = (Guid)itemId;
        var lesson = _fullInclude
            .FirstOrDefault(x => x.Id == id);//?? throw new Exception("Lesson not found");
        lesson.Parents = GetAllChildren(id).ToList();

        if (lesson.Parents != null)
        {
            List<Lesson> parentResult = new List<Lesson>();

            foreach (var par in lesson.Parents)
            {
                parentResult.Add(await _fullInclude.FirstOrDefaultAsync(x => x.Id == (Guid)par.Id));
            }
            lesson.Parents = parentResult;
        }

        return lesson;

    }

    public async Task<Lesson> GetAsync(Guid id)
    {
        var lesson = _fullInclude
            .FirstOrDefault(x => x.Id == id);//?? throw new Exception("Lesson not found");
        lesson.Parents = GetAllChildren(id).ToList();

        if (lesson.Parents != null)
        {
            List<Lesson> parentResult = new List<Lesson>();

            foreach (var par in lesson.Parents)
            {
                parentResult.Add(await _fullInclude.FirstOrDefaultAsync(x => x.Id == (Guid)par.Id));
            }
            lesson.Parents = parentResult;
        }

        return lesson;

    }
    #endregion

    #region LessonStudents

    /*
    public async Task<List<LessonStudent>> UpdateLessonStudents(Lesson lesson, List<Guid> studentsId)
    {
        var groupName = "";
        if (lesson.Group != null) groupName = lesson.Group.Name;
        List<LessonStudent> lessonStudentsByGroup = new List<LessonStudent>();
        if(lesson.ArrivedStudents!=null) lessonStudentsByGroup = lesson.ArrivedStudents.ToList();


        if (lesson.Group != null && lesson.Group.IsPermanentStaffGroup && lesson.IsСompleted) return lessonStudentsByGroup;

        try
        {
            if (lesson != null && lesson.ArrivedStudents != null)
            {
                
                foreach (var student in lessonStudentsByGroup)
                {
                    if (!studentsId.Contains(student.Id))
                    {
                        _context.LessonStudents.Remove(student);

                        try
                        {
                            _context.Students.ToList();
                            var user = await _context.ApplicationUsers.Include(x=>x.UserStudent).FirstOrDefaultAsync(x => x.UserStudent.Id == student.StudentId);

                            var usersToSend = new List<NotificationUser>() { new NotificationUser() { UserId = user.Id } };
                            var message = " You was removed from lesson by group " + groupName + "; Time: "  + lesson.StartTime.ToShortDateString() + " " + lesson.EndTime.ToShortDateString();
                            var notification = new NotificationMessage() { Message = message, Users = usersToSend };

                            usersToSend.ForEach(x => x.NotificationMessage = notification);
                            await _context.NotificationUsers.AddRangeAsync(usersToSend.AsEnumerable());
                            await _context.NotificationMessages.AddAsync(notification);
                            await _context.SaveChangesAsync();
                        }
                        catch { }
                    }
                    try
                    {
                        if (!studentsId.Contains(student.Id))
                        {
                            _context.Students.ToList();
                            _context.ApplicationUsers.ToList();
                           
                           
                           

                        }
                    }
                    catch { }

                    _context.Entry(student).State = EntityState.Detached;
                }

                await _context.SaveChangesAsync();
            }



            //add all
            foreach (var studentId in studentsId)
            {
                if (await _context.LessonStudents.FirstOrDefaultAsync(x => x.LessonId == lesson.Id && x.StudentId == studentId) != null) continue;
                var grSt = new LessonStudent() { LessonId = lesson.Id, StudentId = studentId };
                await _context.LessonStudents.AddAsync(grSt);

                if (lessonStudentsByGroup == null || lessonStudentsByGroup.Count() == 0
                    || (lessonStudentsByGroup != null && lessonStudentsByGroup.Count() != 0 && lessonStudentsByGroup.Select(x => x.StudentId) != null && !lessonStudentsByGroup.Select(x=>x.StudentId).Contains(studentId)))
                {
                    var user = await _context.ApplicationUsers.Include(x => x.UserStudent).FirstOrDefaultAsync(x => x.UserStudent.Id == studentId);

                    var usersToSend = new List<NotificationUser>() { new NotificationUser() { UserId = user.Id } };
                    var message = " You was added to lesson in group " + groupName;
                    var notification = new NotificationMessage() { Message = message, Users = usersToSend };

                    usersToSend.ForEach(x => x.NotificationMessage = notification);
                    await _context.NotificationUsers.AddRangeAsync(usersToSend.AsEnumerable());
                    await _context.NotificationMessages.AddAsync(notification);
                    await _context.SaveChangesAsync();
                }
            }

               
           

            await _context.SaveChangesAsync();
        }
        catch (Exception ex) { }



        return lessonStudentsByGroup;
    }*/
    public async Task DeleteLessonByGroup(Group group, Lesson lesson)
    {
        //_context.Entry(group.Lessons).State = EntityState.Unchanged;
        //var lesson2 = new Lesson() { Id = lesson.Id };
        var students = await _context.LessonStudents.Include(x => x.Student).Where(x => x.Lesson.Id == lesson.Id).ToListAsync();

        foreach (var student in students)
        {
            _context.Entry(student.Student).State = EntityState.Unchanged;
            _context.LessonStudents.Remove(student);
        }


        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task<Lesson> Delete(Lesson lesson, bool isHardDelete = false, bool IsLessonCreatedWithNotifications = true)
    {
        Lesson? result = null;

        if (isHardDelete) result = await TotalDeleteLesson(lesson);

        await _notificationService.СreateToEditLesson(lesson, result);
        return result;
    }

    public async Task<Lesson> TotalDeleteLesson(Lesson lesson) //<Lesson?> 
    {
        //_context.Entry(group.Lessons).State = EntityState.Unchanged;
        //var lesson2 = new Lesson() { Id = lesson.Id };
        var dbLesson = await _context.Lessons.AsNoTracking().FirstOrDefaultAsync(x => x.Id == lesson.Id);
        var parents = GetAllChildren(dbLesson.Id).ToList() ?? new List<Lesson>();
        //parents.Remove(dbLesson);
        //parents =  parents.Insert(0, dbLesson);
        //parents =  parents.Reverse();

        foreach (var lessonDb in parents)
        {
            var students = await _context.LessonStudents.AsNoTracking().Where(x => x.LessonId == lessonDb.Id).ToListAsync();
            if (students != null) _context.LessonStudents.RemoveRange(students);
            await _context.SaveChangesAsync();

            if (lessonDb.TeacherId != null && Guid.Empty != lessonDb.TeacherId)
            {
                var teacher = await _context.LessonTeachers.AsNoTracking().FirstOrDefaultAsync(x => x.LessonId == lessonDb.Id);
                if (teacher != null)
                {
                    _context.LessonTeachers.Remove(teacher);
                    await _context.SaveChangesAsync();
                }
            }




        }
        _context.Lessons.RemoveRange(parents);
        await _context.SaveChangesAsync();

        return null;
    }


    #endregion

    public async Task<Lesson> Create(Lesson lesson, Guid[]? studentId = null, int[]? visitStatus = null, bool IsLessonCreatedWithNotifications = true)
    {
        Guid groupId = (Guid)lesson.GroupId;
        Lesson oldLesson = new Lesson();
        Lesson newLesson = new Lesson();
        var group = await GetGroupAsync(groupId);
        int duration = 0; if (group != null) duration = group.LessonType.LessonTimeInMinutes;

        newLesson = await base.CreateAsync(lesson);
        await UpdateLessonStudents(oldLesson, newLesson, studentId?.ToList(), visitStatus, IsLessonCreatedWithNotifications);
        await UpdateLessonTeachers(oldLesson, newLesson, new List<Guid> { (Guid)lesson.TeacherId }, IsLessonCreatedWithNotifications);

        return lesson;

    }

    public async Task<Lesson> Edit(Lesson lesson, Guid[]? studentId, int[]? visitStatus, bool IsLessonCreatedWithNotifications = true)
    {
        Guid groupId = (Guid)lesson.GroupId;
        Lesson oldLesson = await GetAsync(lesson.Id);
        Lesson newLesson = new Lesson();
        var group = await GetGroupAsync(lesson.GroupId ?? groupId);
        //int previousCountLessons = group.Lessons.Count();
        var user = await _userService.GetCurrentUserAsync();


        if (await _userManager.IsInRoleAsync(user, "Teacher") && !(await _userManager.IsInRoleAsync(user, "Admin"))) //условие для учителей
        {
            /*
            lesson.StartTime = prevLesson.StartTime;
            lesson.EndTime = prevLesson.EndTime;*/
            //var prevLesson = await _context.Lessons.AsNoTracking().Include(x => x.Group).ThenInclude(x => x.LessonType).ThenInclude(x => x.Course).FirstOrDefaultAsync(x => x.Id == lesson.Id);
            lesson.StartTime = oldLesson.StartTime;
            lesson.EndTime = oldLesson.EndTime;
            //_context.Entry(prevLesson).State = EntityState.Detached;

            /*
            var requestMessage = user.Login.ToString()
            + " want to update current lesson with values: "
            + "\nStart time : " + lesson.StartTime.ToShortDateString() + " " + lesson.StartTime.ToShortTimeString()
            + "\nEnd time : " + lesson.EndTime.ToShortDateString() + " " + lesson.EndTime.ToShortTimeString();
            await CreateRequest(prevLesson, requestMessage, lesson, user);
            await _notificationService.СreateToEditLesson(prevLesson, lesson, null);
            await _context.SaveChangesAsync();*/

            //return lesson;
        }
        newLesson = await UpdateAsync(lesson);
        if (oldLesson.StartTime != newLesson.StartTime || oldLesson.EndTime != newLesson.EndTime)
        {
            await _notificationService.СreateToEditLesson(oldLesson, newLesson);
        }
        //await _notificationService.СreateToUpdateCountLessonsInGroup(group, previousCountLessons, previousCountLessons + 1, null);

        await UpdateLessonStudents(oldLesson, newLesson, studentId.ToList(), visitStatus, IsLessonCreatedWithNotifications);
        await UpdateLessonTeachers(oldLesson, newLesson, new List<Guid> { (Guid)lesson.TeacherId }, IsLessonCreatedWithNotifications);


        return lesson;

    }

    public async Task<List<Lesson>> CreateLessonsBySchedule(Group group, int countLessons, bool isVerified = true, bool IsLessonCreatedWithNotifications = true)
    {
        var lessons = new List<Lesson>();
        if (group == null || group.Id == Guid.Empty) return null;
        var groupDb = _context.Groups.Include(x => x.DaySchedules).Include(x => x.GroupStudents).FirstOrDefault(x => x.Id == group.Id);
        var schedules = groupDb?.DaySchedules;
        var distinctSchedule = schedules.Select(x => x.DayName).Distinct().ToList();
        var groupStudents = groupDb?.GroupStudents;
        var groupLessons = _context.Lessons.Include(x => x.Group).Where(x => x.GroupId == group.Id);

        if (group.DateStart == DateTime.MinValue || schedules.Count() < 1) return null;

        ////////////////////////////// 
        ///Мы берем день начала курса - currentDate
        ///Мы берем расписание
        ///Бегаем по currentDate. Если currentDate.DayOfWeek == DayOfWeek из расписания и нет занятия в этой группе -> создаем занятие. Если занятие есть  - пропускаем 
        ///Проверка при добавлении ученика. Если у ученика есть другие занятия во время занятия -> нельзя добавить ученика в группу. Сообщить что расписание ученика

        var currentDate = DateTime.Today;
        if (group.DateStart > currentDate) currentDate = group.DateStart;
        var count = 0;
        var countCheckedScheduleDays = 0;




        while (count < countLessons)
        {
            if (!distinctSchedule.Contains(currentDate.DayOfWeek))
            {
                //if (countCheckedScheduleDays < schedules.Count) 
                currentDate = currentDate.AddDays(1);
                //else { currentDate = currentDate.AddDays(7); countCheckedScheduleDays = 0; }
                continue;
            }
            foreach (var scheduleDay in schedules)
            {
                if (countLessons == count) break;
                if (currentDate.DayOfWeek != scheduleDay.DayName) { continue; }


                DateTime lessonStartTime = currentDate.Date + (TimeSpan)scheduleDay.WorkingStartTime;
                DateTime lessonEndTime = currentDate.Date + (TimeSpan)scheduleDay.WorkingEndTime;

                // Создание занятия
                var lesson = new Lesson
                {
                    StartTime = lessonStartTime,
                    EndTime = lessonEndTime,
                    GroupId = group.Id, ///
                    TeacherId = _context.GroupTeachers.FirstOrDefault(x => x.GroupId == group.Id).TeacherId
                };
                if (groupLessons == null || groupLessons.Any(x => x.StartTime == lesson.StartTime && x.EndTime == lesson.EndTime)) { continue; }


                lessons.Add(lesson);
                if (group.IsPermanentStaffGroup) await Create(lesson, groupStudents.Select(x => x.StudentId).ToArray());
                else await Create(lesson);
                count++;

            }
            countCheckedScheduleDays++; // Инкрементирование countCheckedScheduleDays

            //if (countCheckedScheduleDays < schedules.Count) 
            currentDate = currentDate.AddDays(1);
            //else { currentDate = currentDate.AddDays(7); countCheckedScheduleDays = 0; }
        }

        //if (groupLessons != null && groupLessons.Count() > 0) group.DateStart = groupLessons.OrderBy(x=>x.EndTime).FirstOrDefault().EndTime;
        /*
        try
        {
            var date = startDate;//hz
            if (countLessons < 0) countLessons = 100;
            for (int lesCount = 0, scCount = 0; lesCount < countLessons; lesCount++, scCount++)
            {
                /////
                if (scCount == schedules.Count)
                {
                    scCount = 0;
                    date = date.AddDays(7);
                }
                var addingDays = LessonMath.Mod((int)date.DayOfWeek, (int)schedules[scCount].DayName);
                var virtualDate = date.AddDays(addingDays);

                /////


                var lesson = new Lesson()
                {
                    StartTime = virtualDate + schedules[scCount].WorkingStartTime ?? DateTime.Now,
                    EndTime = virtualDate + schedules[scCount].WorkingEndTime ?? DateTime.Now,
                    GroupId = group.Id, ///
                    TeacherId = group.TeacherId
                };
                if (groupLessons == null || groupLessons.Any(x => x.StartTime == lesson.StartTime && x.EndTime == lesson.EndTime)) continue;//throw new Exception("Lesson with same datetime already exist");

                //lessons.Add(lesson);
                Guid[] studentsIds = new Guid[0];
                if (group.IsPermanentStaffGroup && group.GroupStudents != null && group.GroupStudents.Count() > 0)
                {
                    studentsIds = group.GroupStudents.Select(x => x.StudentId).ToArray();
                }
                int[] filledArray = Enumerable.Repeat(1, studentsIds.Length).ToArray();
                await Create(lesson, studentsIds, filledArray, IsLessonCreatedWithNotifications);



            }
        } catch (Exception ex) { }
    */
        return lessons;
    }
    //public async Task<RequestLesson> CreateRequest(Lesson prevLesson, string requestMessage, Lesson newLesson, ApplicationUser user)
    //{
    //    DateTime newStart = DateTime.Now;
    //    DateTime newEnd = DateTime.Now;
    //    if (newLesson != null)
    //    {
    //        var duration = (newLesson.EndTime - newLesson.StartTime).TotalMinutes;
    //        var defaultDuration = prevLesson.Group.LessonType.LessonTimeInMinutes;
    //        requestMessage += "\n| Default time to lesson type " + prevLesson.Group.Name + " : " + defaultDuration
    //            + "\n | New duration : " + duration
    //            + "\n Difference : " + (defaultDuration - duration);

    //        newStart = newLesson.StartTime;
    //        newEnd = newLesson.EndTime;

    //    }
    //    else
    //    {
    //        requestMessage += " in group '" + newLesson.Group.Name + "' ";//+ "' lesson  was deleted by date:  ";
    //                                                                      // + lesson.StartTime.ToShortTimeString() + " - " + lesson.EndTime.ToShortTimeString();
    //    }

    //    //var request = new RequestLesson() { LessonBefore = newLesson, RequestMessage = requestMessage, User = user, NewStart = newStart, NewEnd = newEnd };

    //    _context.Entry(request.User).State = EntityState.Unchanged;
    //    _context.Entry(request.LessonBefore).State = EntityState.Unchanged;

    //    //_context.RequestLessons.Add(request);
    //    await _context.SaveChangesAsync();
    //    return request;
    //}
    public override async Task Validate(Lesson oldValue, Lesson newValue)
    {
        await base.Validate(oldValue, newValue);

        Guid groupId = (Guid)newValue.GroupId;
        var group = await GetGroupAsync(groupId);
        int duration = 0;
        if (group != null) duration = group.LessonType.LessonTimeInMinutes;
        if (newValue.EndTime.Minute - newValue.StartTime.Minute > duration * 2 || newValue.EndTime < newValue.StartTime) throw new Exception("Not correct date");

        var lessonsByGroup = _context.Lessons.Include(x => x.Group).Where(x => x.Group.Id == groupId).OrderBy(x => x.StartTime);
        /*foreach (var less in lessonsByGroup)
        {
            if (less.Id == lesson.Id) continue;
            if (lesson.StartTime.CompareTo(less.StartTime) >= 0 && lesson.EndTime.CompareTo(less.EndTime) <= 0)
            {
                throw new Exception("New lesson time conflicted with lesson :" + less.StartTime + " - " + less.EndTime);
            }
        }*/
    }
    public async Task<Group> GetGroupAsync(Guid id)
    {
        var groups = await _context.Groups
            //.Include(x => x.Lessons)
            //.Include(x => x.Lessons).ThenInclude(x => x.Teacher).ThenInclude(x => x.Teacher).ThenInclude(x => x.ApplicationUser)
            //.Include(x => x.Lessons).ThenInclude(x => x.ArrivedStudents).ThenInclude(x => x.Student).ThenInclude(x => x.ApplicationUser)
            //.Include(x => x.Lessons)
            //.Include(x => x.GroupStudents).ThenInclude(x => x.Group)
            //.Include(x => x.GroupStudents).ThenInclude(x => x.Student).ThenInclude(x => x.ApplicationUser)
            //.Include(x => x.GroupStudents).ThenInclude(x => x.Group).ThenInclude(x => x.Lessons)
            .Include(x => x.GroupTeachers).ThenInclude(x => x.Teacher).ThenInclude(x => x.ApplicationUser).ThenInclude(x => x.ConnectedUsersInfo).ThenInclude(x => x.BaseUserInfo)
            .Include(x => x.GroupStudents).ThenInclude(x => x.Student).ThenInclude(x => x.ApplicationUser).ThenInclude(x => x.ConnectedUsersInfo).ThenInclude(x => x.BaseUserInfo)
            .Include(x => x.DaySchedules)
            .Include(x => x.LessonType).ThenInclude(x => x.AgeType)
            .Include(x => x.LessonType).ThenInclude(x => x.Course)
            .Include(x => x.LessonType).ThenInclude(x => x.LessonTypePaymentCategory).ThenInclude(x => x.PaymentCategory)
            .Include(x => x.LessonType).ThenInclude(x => x.GroupType)
            .Include(x => x.LessonType).ThenInclude(x => x.Location)
            .Include(x => x.LessonType).ThenInclude(x => x.AgeType)
            .Include(x => x.PaymentCategory)
            .ToListAsync();
        var item = groups.Find(x => x.Id == id);
        return item;
    }

    public async Task<Lesson> UpdateLessonStudents(Lesson oldLesson, Lesson newLesson, List<Guid>? studentsId, int[]? visitStatus, bool IsLessonCreatedWithNotifications = true)
    {
        var oldStudents = new List<Guid>();
        studentsId = studentsId ?? new List<Guid>();
        visitStatus = visitStatus ?? new int[studentsId.Count].Select(x => 1).ToArray();
        if (studentsId.Count() == 0 && newLesson.GroupId != null)
        {
            var gr = _context.Groups.Include(x => x.GroupStudents).FirstOrDefault(x => x.Id == newLesson.GroupId);
            if (gr != null && gr.IsPermanentStaffGroup)
            {
                studentsId = gr?.GroupStudents
                .Where(x => !x.IsDeleted) // Фильтрация по IsDeleted == false
                .GroupBy(x => new { x.StudentId, x.DateCreated, x.IsDeleted }) // Группировка по 3 полям
                .Select(g => g.OrderByDescending(x => x.DateCreated).First()) // Выбор первого элемента по группам, сортировка по убыванию DateCreated
                .ToList()
                .Select(x => x.StudentId).ToList();
                visitStatus = new int[studentsId.Count].Select(x => x).ToArray();
            }

        }


        if (oldLesson.Id != Guid.Empty) oldStudents = await _context.LessonStudents.Where(x => x.LessonId == oldLesson.Id).Select(x => x.StudentId).ToListAsync();
        List<Guid> toCreate = studentsId.Except(oldStudents).ToList();
        List<Guid> toUpdate = studentsId.Intersect(oldStudents).ToList();
        List<Guid> toDelete = oldStudents.Except(studentsId).ToList();



        foreach (var studentId in toCreate)
        {

            var student = await _context.Students.FindAsync(studentId);
            if (student == null) continue;
            var visitIndex = studentsId.IndexOf(studentId);
            var visit = (int)visitStatus.GetValue(visitIndex);

            var lessonSt = new LessonStudent() { LessonId = newLesson.Id, StudentId = studentId, VisitStatus = visit };
            await _context.LessonStudents.AddAsync(lessonSt);
            await _context.SaveChangesAsync();

            if (!IsLessonCreatedWithNotifications) continue;

            #region Create notification when user was added to lesson
            var usersToSend = new List<NotificationUser>() { new NotificationUser() { UserId = student.ApplicationUserId } };
            var message = " You was added to lesson " + newLesson.StartTime.ToShortDateString() + " in group " + newLesson.Group?.Name;
            var notification = new NotificationMessage() { Message = message, Users = usersToSend };

            usersToSend.ForEach(x => x.NotificationMessage = notification);
            await _context.NotificationUsers.AddRangeAsync(usersToSend.AsEnumerable());
            await _context.NotificationMessages.AddAsync(notification);
            await _context.SaveChangesAsync();
            #endregion
        }
        foreach (var studentId in toUpdate)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null) continue;

            var visitIndex = studentsId.IndexOf(studentId);
            var visit = (int)visitStatus.GetValue(visitIndex);

            //var dbLessonStudent = _context.LessonStudents.FirstOrDefaultAsync(x => x.StudentId == studentId && x.LessonId == oldLesson.Id);
            var grSt = new LessonStudent() { LessonId = newLesson.Id, StudentId = studentId, VisitStatus = visit };
            await _context.LessonStudents.AddAsync(grSt);
            await _context.SaveChangesAsync();
        }
        foreach (var studentId in toDelete)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null) continue;
            var grSt = new LessonStudent() { LessonId = newLesson.Id, StudentId = studentId, VisitStatus = 3, IsDeleted = true };
            await _context.LessonStudents.AddAsync(grSt);
            await _context.SaveChangesAsync();
            if (!IsLessonCreatedWithNotifications) continue;
            #region Create notification to remove from group
            try
            {
                var stud = await _context.Students.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.Id == student.Id);
                var usersToSend = new List<NotificationUser>() { new NotificationUser() { UserId = student.ApplicationUserId } };
                var message = " You was removed from lesson " + newLesson.StartTime.ToShortDateString() + " in group " + newLesson.Group?.Name;
                var notification = new NotificationMessage() { Message = message, Users = usersToSend };

                usersToSend.ForEach(x => x.NotificationMessage = notification);
                await _context.NotificationUsers.AddRangeAsync(usersToSend.AsEnumerable());
                await _context.NotificationMessages.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
            catch { }
            #endregion
        }
        await _context.SaveChangesAsync();
        return await GetAsync(newLesson.Id);

    }

    public async Task CheckCorrectWorkingTime(ApplicationUser user, DateTime startTime, DateTime endTime)
    {
        Student? student = user.UserStudent;
        Teacher? teacher = user.UserTeacher;
        if (student != null)
        {
            var groups = _context.Groups
                .Include(x => x.GroupStudents)
                .Where(x => x.GroupStudents != null && x.GroupStudents.Select(x => x.StudentId).Contains(student.Id))
                .Include(x => x.DaySchedules);//_context.GroupStudents.Where(x => x.ParentId == null).Where(x => x.StudentId == student.Id);
            foreach (var group in groups)
            {
                if (group.DaySchedules == null) continue;
                foreach (var schedule in group.DaySchedules)
                {
                    if (startTime.DayOfWeek == schedule.DayName && endTime.DayOfWeek == schedule.DayName
                        && !(startTime.TimeOfDay > schedule.WorkingEndTime || endTime.TimeOfDay < schedule.WorkingStartTime))
                        throw new ApplicationException($"In group {group.Name} day of schedule - {schedule.DayNameString} {schedule.WorkingStartTime.Value} {schedule.WorkingEndTime.Value} intersect with {startTime.DayOfWeek.ToString()} {startTime.TimeOfDay.ToString()} {schedule.WorkingEndTime.Value} intersect with {startTime.DayOfWeek.ToString()} {endTime.TimeOfDay.ToString()}");

                }
            }
        }
        if (teacher != null)
        {
            var groups = _context.Groups
                .Include(x => x.GroupTeachers)
                .Where(x => x.GroupTeachers != null && x.GroupTeachers.Select(x => x.TeacherId).Contains(teacher.Id))
                .Include(x => x.DaySchedules);//_context.GroupStudents.Where(x => x.ParentId == null).Where(x => x.StudentId == student.Id);
            foreach (var group in groups)
            {
                if (group.DaySchedules == null) continue;
                foreach (var schedule in group.DaySchedules)
                {
                    if (startTime.DayOfWeek == schedule.DayName && endTime.DayOfWeek == schedule.DayName
                        && !(startTime.TimeOfDay > schedule.WorkingEndTime || endTime.TimeOfDay < schedule.WorkingStartTime))
                        throw new Exception($"In group {group.Name} day of schedule - {schedule.DayNameString} {schedule.WorkingStartTime.Value} {schedule.WorkingEndTime.Value} intersect with {startTime.DayOfWeek.ToString()} {startTime.TimeOfDay.ToString()} {schedule.WorkingEndTime.Value} intersect with {startTime.DayOfWeek.ToString()} {endTime.TimeOfDay.ToString()}");

                }
            }
        }
    }

    public async Task<Lesson> UpdateLessonTeachers(Lesson oldLesson, Lesson newLesson, List<Guid> teachersId, bool IsLessonCreatedWithNotifications = true)
    {
        var oldTeachers = new List<Guid>();

        if (oldLesson.Id != Guid.Empty) oldTeachers = await _context.LessonTeachers.Where(x => x.LessonId == oldLesson.Id).Select(x => x.TeacherId).ToListAsync();
        List<Guid> toCreate = teachersId.Except(oldTeachers).ToList();
        List<Guid> toUpdate = teachersId.Intersect(oldTeachers).ToList();
        List<Guid> toDelete = oldTeachers.Except(teachersId).ToList();


        foreach (var studentId in toCreate)
        {
            var student = await _context.Teachers.FindAsync(studentId);
            if (student == null) continue;

            var lessonSt = new LessonTeacher() { LessonId = newLesson.Id, TeacherId = studentId };
            await _context.LessonTeachers.AddAsync(lessonSt);
            await _context.SaveChangesAsync();

            if (!IsLessonCreatedWithNotifications) continue;
            #region Create notification when user was added to lesson
            var usersToSend = new List<NotificationUser>() { new NotificationUser() { UserId = student.ApplicationUserId } };
            var message = " You was added to lesson " + newLesson.StartTime.ToShortDateString() + " in group " + newLesson.Group?.Name;
            var notification = new NotificationMessage() { Message = message, Users = usersToSend };

            usersToSend.ForEach(x => x.NotificationMessage = notification);
            await _context.NotificationUsers.AddRangeAsync(usersToSend.AsEnumerable());
            await _context.NotificationMessages.AddAsync(notification);
            await _context.SaveChangesAsync();
            #endregion
        }
        foreach (var studentId in toUpdate)
        {
            var student = await _context.Teachers.FindAsync(studentId);
            if (student == null) continue;

            var grSt = new LessonTeacher() { LessonId = newLesson.Id, TeacherId = studentId };
            await _context.LessonTeachers.AddAsync(grSt);
            await _context.SaveChangesAsync();
        }
        foreach (var studentId in toDelete)
        {
            var student = await _context.Teachers.FindAsync(studentId);
            if (student == null) continue;
            if (!IsLessonCreatedWithNotifications) continue;
            #region Create notification to remove from group
            try
            {
                var stud = await _context.Teachers.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.Id == student.Id);
                var usersToSend = new List<NotificationUser>() { new NotificationUser() { UserId = student.ApplicationUserId } };
                var message = " You was removed from lesson " + newLesson.StartTime.ToShortDateString() + " in group " + newLesson.Group?.Name;
                var notification = new NotificationMessage() { Message = message, Users = usersToSend };

                usersToSend.ForEach(x => x.NotificationMessage = notification);
                await _context.NotificationUsers.AddRangeAsync(usersToSend.AsEnumerable());
                await _context.NotificationMessages.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
            catch { }
            #endregion
        }
        await _context.SaveChangesAsync();
        return newLesson;
    }

    public async Task UpdateLessonsUsersForUnCompletedLessonsByGroupAsync(Guid groupId, Guid? newTeacherId = null, List<Guid> newStudents = null)
    {
        var notCompletedLessons = await _context.Lessons
            .Where(x => x.GroupId == groupId && x.ParentId == null).Where(x => !x.IsСompleted || x.EndTime < DateTime.Now)
            .Include(x => x.Group).ThenInclude(x => x.GroupTeachers)
            .Include(x => x.Group).ThenInclude(x => x.GroupStudents)
            .Include(x => x.Teacher)
            .Include(x => x.ArrivedStudents)
            .ToListAsync();
        newTeacherId = newTeacherId ?? notCompletedLessons.First().Group?.TeacherId;
        newStudents = newStudents ?? notCompletedLessons?.First()?.Group?.GroupStudents?.Select(x => x.StudentId)?.ToList();
        foreach (var x2 in notCompletedLessons)
        {

            x2.TeacherId = newTeacherId;
            var students = _context.Lessons.Include(x => x.ArrivedStudents).SelectMany(x => x.ArrivedStudents);
            var dbStudentsIds = students.Select(x => x.StudentId)?.ToList();
            var dbVisitStatuses = students.Select(x => x.VisitStatus)?.ToList();


            List<Guid> toCreate = newStudents.Except(dbStudentsIds).ToList();
            List<Guid> toUpdate = newStudents.Intersect(dbStudentsIds).ToList();
            List<Guid> toDelete = dbStudentsIds.Except(newStudents).ToList();

            List<Guid> combinedList = toUpdate.Concat(toCreate).ToList();
            int index = toUpdate.Count();
            foreach (var id in combinedList)
            {
                if (toCreate.Contains(id))
                {
                    dbVisitStatuses.Insert(index, 1);
                }
                index++;
            }

            await Edit(x2, combinedList.ToArray(), dbVisitStatuses.ToArray(), false);

        }


    }
}

