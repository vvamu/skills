using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using skills_hub.domain.Models.User;
using skills_hub.persistence;
using skills_hub.tests.Helpers;

namespace skills_hub.tests;

public class UserServiceTest : IDisposable
{

    private ApplicationDbContext? _db;
    private Mock<FakeUserManager> _mockUserManager;
    private Mock<FakeRoleManager> _mockRoleManager;
    [SetUp]
    public void Setup()
    {
        var dbOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("in_memory_db");
        _db = new ApplicationDbContext(dbOptionsBuilder.Options);

        _mockUserManager = new Mock<FakeUserManager>(); //IAbstractLogModel<BaseUserInfo
        _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(x => x.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(new bool());
        _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string>());

        _mockRoleManager = new Mock<FakeRoleManager>();
        var teacherRole = new IdentityRole<Guid>("Teacher");
        var studentRole = new IdentityRole<Guid>("Student");

        _mockRoleManager.Setup(r => r.CreateAsync(It.IsAny<IdentityRole<Guid>>())).ReturnsAsync(IdentityResult.Success);
        _mockRoleManager.Setup(r => r.CreateAsync(teacherRole)).ReturnsAsync(IdentityResult.Success);
        _mockRoleManager.Setup(r => r.CreateAsync(studentRole)).ReturnsAsync(IdentityResult.Success);


        _mockRoleManager.Setup(r => r.Roles).Returns(new List<IdentityRole<Guid>> { teacherRole, studentRole }.AsQueryable());
    }
    public void Dispose()
    {
        _db.Dispose();
    }
    [Test]
    public async Task Create()
    {
        var dbOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("in_memory_db");

        //var sut = new skills_hub.core.Repository.User.UserService(_db, _mockUserManager.Object, _mockRoleManager.Object);
        //var result = await sut.();
        //var u = await sut.CreateUserAsync();
        //result = await sut.GetUsersAsync();

        Assert.Equals(new DateTime(1594155600), new DateTime(1594155600));

    }
}