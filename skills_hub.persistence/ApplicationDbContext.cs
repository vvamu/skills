//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using skills_hub.domain.Models;
using skills_hub.domain.Models.Groups;
using skills_hub.domain.Models.LessonTypes;
using skills_hub.domain.Models.ManyToMany;
using skills_hub.domain.Models.User;
using System.Text.RegularExpressions;

namespace skills_hub.persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<domain.Models.Groups.Group> Groups { get; set; }
    public DbSet<Lesson> Lessons { get; set; }

    public DbSet<LessonType> LessonTypes { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<GroupType> GroupTypes { get; set; }
    public DbSet<AgeType> AgeTypes { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<PaymentCategory> PaymentCategories { get; set; }
    public DbSet<EmailMessage> EmailMessages { get; set; }

    public DbSet<NotificationMessage> NotificationMessages { get; set; }
    public DbSet<NotificationUser> NotificationUsers { get; set; }

    #region Users

    public DbSet<BaseUserInfo> BaseUserInfo { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }


    #region ManyToMany
    public DbSet<GroupStudent> GroupStudents { get; set; }
    public DbSet<GroupTeacher> GroupTeachers { get; set; }
    public DbSet<LessonStudent> LessonStudents { get; set; }
    public DbSet<LessonTeacher> LessonTeachers { get; set; }
    public DbSet<ApplicationUserBaseUserInfo> ApplicationUserBaseUserInfo { get; set; }
    public DbSet<LessonTypePaymentCategory> LessonTypePaymentCategories { get; set; }



    #endregion
    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {

        builder.Ignore<IdentityUserToken<Guid>>();
        builder.Ignore<IdentityUserLogin<Guid>>();
        builder.Ignore<IdentityUserClaim<Guid>>();
        builder.Ignore<IdentityRoleClaim<Guid>>();

        #region User
        builder.Entity<ApplicationUser>().HasOne(x => x.UserTeacher).WithOne(x => x.ApplicationUser).HasForeignKey<Teacher>(x => x.ApplicationUserId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<ApplicationUser>().HasOne(x => x.UserStudent).WithOne(x => x.ApplicationUser).HasForeignKey<Student>(x => x.ApplicationUserId).OnDelete(DeleteBehavior.Cascade);
        /*
        builder.Entity<Teacher>().HasOne(x=>x.ApplicationUser).WithOne(x => x.UserTeacher).HasForeignKey<ApplicationUser>(x=>x.UserTeacherId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Student>().HasOne(x => x.ApplicationUser).WithOne(x => x.UserStudent).HasForeignKey<St(x => x.UserStudentId).OnDelete(DeleteBehavior.Cascade);
        */

        #region ManyToMany
        builder.Entity<GroupTeacher>().HasKey(ct => new { ct.GroupId, ct.TeacherId });
        builder.Entity<GroupTeacher>().HasOne(x => x.Teacher).WithMany(x => x.Groups).HasForeignKey(x => x.TeacherId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<GroupTeacher>().HasOne(x => x.Group).WithMany(x => x.GroupTeachers).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<LessonTeacher>().HasKey(ct => new { ct.LessonId, ct.TeacherId });
        builder.Entity<LessonTeacher>().HasOne(x => x.Teacher).WithMany(x => x.Lessons).HasForeignKey(x => x.TeacherId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<LessonTeacher>().HasOne(x => x.Lesson).WithOne(x => x.Teacher).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<LessonStudent>().HasKey(ct => new { ct.LessonId, ct.StudentId });
        builder.Entity<LessonStudent>().HasOne(x => x.Student).WithMany(x => x.Lessons).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<LessonStudent>().HasOne(x => x.Lesson).WithMany(x => x.ArrivedStudents).HasForeignKey(x => x.LessonId).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GroupStudent>().HasKey(ct => new { ct.GroupId, ct.StudentId });
        builder.Entity<GroupStudent>().HasOne(x => x.Student).WithMany(x => x.Groups).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<GroupStudent>().HasOne(x => x.Group).WithMany(x => x.GroupStudents).HasForeignKey(x => x.GroupId).OnDelete(DeleteBehavior.Cascade);

        #endregion

        #endregion

        builder.Entity<GroupWorkingDay>().HasOne(x => x.Group).WithMany(x => x.DaySchedules).OnDelete(DeleteBehavior.NoAction);
        //builder.Entity<WorkingDay>().HasOne(x => x.Student).WithMany(x => x.WorkingDays).OnDelete(DeleteBehavior.NoAction);
        //builder.Entity<WorkingDay>().HasOne(x => x.Teacher).WithMany(x => x.WorkingDays).OnDelete(DeleteBehavior.NoAction);


        //builder.Entity<Group>().HasOne(x => x.Teacher).WithMany(x => x.Groups).OnDelete(DeleteBehavior.SetNull);


        builder.Entity<Lesson>().HasMany(x => x.ArrivedStudents).WithOne(x => x.Lesson).OnDelete(DeleteBehavior.SetNull);

        #region Many-to-many
        builder.Entity<ApplicationUserBaseUserInfo>().HasOne(x => x.ApplicationUser).WithMany(x => x.ConnectedUsersInfo).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<ApplicationUserBaseUserInfo>().HasOne(x => x.BaseUserInfo).WithMany(x => x.ApplicationUsers).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<BaseUserInfo>().HasOne(b => b.Parent).WithMany().HasForeignKey(b => b.ParentId).OnDelete(DeleteBehavior.Cascade);

        /*
        builder.Entity<CourseNameTeacher>()
       .HasKey(ct => new { ct.CourseNameId, ct.TeacherId });

        builder.Entity<CourseNameTeacher>()
            .HasOne(ct => ct.CourseName)
            .WithMany(c => c.Teachers)
            .HasForeignKey(ct => ct.CourseNameId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete for CourceNameTeacher

        builder.Entity<CourseNameTeacher>()
            .HasOne(ct => ct.Teacher)
            .WithMany(t => t.PossibleCources)
            .HasForeignKey(ct => ct.TeacherId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete for CourceNameTeacher

        builder.Entity<CourseStudent>()
       .HasKey(ct => new { ct.CourseNameId, ct.StudentId });

        builder.Entity<CourseStudent>()
            .HasOne(ct => ct.CourseName)
            .WithMany(c => c.Students)
            .HasForeignKey(ct => ct.CourseNameId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete for CourceNameTeacher

        builder.Entity<CourseStudent>()
            .HasOne(ct => ct.Student)
            .WithMany(t => t.PossibleCources)
            .HasForeignKey(ct => ct.StudentId)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete for CourceNameTeacher

        */


        builder.Entity<NotificationUser>()
            .HasOne(x => x.User)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<NotificationUser>()
            .HasOne(x => x.NotificationMessage)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.NotificationMessageId)
            .OnDelete(DeleteBehavior.Cascade);



        #endregion

        base.OnModelCreating(builder);
    }


}