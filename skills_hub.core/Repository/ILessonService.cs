namespace skills_hub.core.repository;

public interface ILessonService : IAbstractLogModel<Lesson>
{
    public IQueryable<Lesson> GetAll();
    public IQueryable<Lesson> GetGroupLessonsList();
    public Task<Lesson> GetAsync(Guid id);

    public Task<Lesson> Create(Lesson lesson, Guid[]? studentId, int[]? visitStatus, bool IsLessonCreatedWithNotifications = true);
    public Task<Lesson> Edit(Lesson lesson, Guid[]? studentId, int[]? visitStatus, bool IsLessonCreatedWithNotifications = true);
    public Task DeleteLessonByGroup(Group group, Lesson lesson);

    public Task<Lesson> Delete(Lesson lesson, bool isHardDelete = false, bool IsLessonCreatedWithNotifications = true);
    //public Task TotalDeleteLesson(Lesson lesson);
    //public Task<List<LessonStudent>> UpdateLessonStudents(Lesson lesson, List<Guid> studentIds);

    public Task<List<Lesson>> CreateLessonsBySchedule(Group group, int countLessons, bool isVerified = true, bool IsLessonCreatedWithNotifications = true);
    //public Task<LessonStudent> UpdateLessonStudent(Lesson lesson, Guid studentId);

    // public Task<List<LessonStudent>> CreateLessonStudents(List<Lesson> lessons, List<Guid> studentIds, Teacher teacher);

    public Task UpdateLessonsUsersForUnCompletedLessonsByGroupAsync(Guid groupId, Guid? newTeacherId = null, List<Guid> newStudents = null);



}