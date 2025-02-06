//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using skills_hub.core.Repository.LessonType;
//using skills_hub.domain.Models.LessonTypes;
//using skills_hub.domain.Models.User;
//using skills_hub.persistence;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System.Security.Claims;

//namespace skills_hub.tests;

//public class Tests
//{
//    [Test]
//    public async Task Returns_Expected_Values_From_the_Api()
//    {
//        var dbOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("in_memory_db");

//        var roleManagerMock = new Mock<RoleManager<IdentityRole<Guid>>>(
//        new Mock<IRoleStore<IdentityRole<Guid>>>().Object,
//        new IRoleValidator<IdentityRole<Guid>>[0],
//        new Mock<ILookupNormalizer>().Object,
//        new Mock<IdentityErrorDescriber>().Object,
//        new Mock<ILogger<RoleManager<IdentityRole<Guid>>>>().Object);

//        var teacherRole = new IdentityRole<Guid>("Teacher");
//        var studentRole = new IdentityRole<Guid>("Student");

//        roleManagerMock.Setup(r => r.CreateAsync(It.IsAny<IdentityRole<Guid>>())).ReturnsAsync(IdentityResult.Success);
//        roleManagerMock.Setup(r => r.CreateAsync(teacherRole)).ReturnsAsync(IdentityResult.Success);
//        roleManagerMock.Setup(r => r.CreateAsync(studentRole)).ReturnsAsync(IdentityResult.Success);

        
//        roleManagerMock.Setup(r => r.Roles).Returns(new List<IdentityRole<Guid>> { teacherRole, studentRole }.AsQueryable());
        
//        using (var db = new ApplicationDbContext(dbOptionsBuilder.Options))
//        {
//            var fakeUserManager = new Mock<FakeUserManager>();
            


//            fakeUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
//            fakeUserManager.Setup(x => x.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync( new bool());
//            fakeUserManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string>());


//            var sut = new AgeTypeServices(db, fakeUserManager.Object, roleManagerMock.Object);
//            var result = await sut.GetUsersAsync();
//            var u = await sut.CreateUserAsync();
//            result = await sut.GetUsersAsync();

//            Assert.Equals(new DateTime(1594155600), new DateTime(1594155600));
//        }
//    }
//}

//public class FakeUserManager : UserManager<ApplicationUser>
//{
//    public FakeUserManager()
//        : base(new Mock<IUserStore<ApplicationUser>>().Object,
//              new Mock<IOptions<IdentityOptions>>().Object,
//              new Mock<IPasswordHasher<ApplicationUser>>().Object,
//              new IUserValidator<ApplicationUser>[0],
//              new IPasswordValidator<ApplicationUser>[0],
//              new Mock<ILookupNormalizer>().Object,
//              new Mock<IdentityErrorDescriber>().Object,
//              new Mock<IServiceProvider>().Object,
//              new Mock<ILogger<UserManager<ApplicationUser>>>().Object)
//    { }

//}