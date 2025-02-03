namespace skills_hub.core.Repository.User.Interfaces;

public interface INotificationService
{
    public Task CreateToCreateGroup(Group group, List<ApplicationUser> teachers = null, List<ApplicationUser> students = null, List<ApplicationUser> admins = null);
    public Task CreateToRemoveGroup(Group group, List<ApplicationUser> teachers = null, List<ApplicationUser> students = null, List<ApplicationUser> admins = null);
    public Task CreateToAddUsersToGroup(Group group, List<ApplicationUser> teachers = null, List<ApplicationUser> students = null, List<ApplicationUser> admins = null);
    public Task CreateToRemoveUsersFromGroup(Group group, List<ApplicationUser> teachers = null, List<ApplicationUser> students = null, List<ApplicationUser> admins = null);

    public Task<NotificationMessage> СreateToEditLesson(Lesson lastLessonValue, Lesson? newlessonValue = null, List<ApplicationUser>? usersToSend = null, int answer = 1);
    public Task<NotificationMessage> СreateToUpdateCountLessonsInGroup(Group group, int previousCountLessons, int currentCountLessons, List<ApplicationUser>? usersToSend);

    public Task<NotificationMessage> Create(string message, List<ApplicationUser>? usersToSend);

}
