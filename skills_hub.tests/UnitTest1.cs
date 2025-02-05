using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using skills_hub.core.Repository.LessonType;
using skills_hub.domain.Models.LessonTypes;
using skills_hub.domain.Models.User;
using skills_hub.persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace skills_hub.tests;

public class Tests
{
    [Test]
    public async Task Returns_Expected_Values_From_the_Api()
    {
        //var opts = OptionsBuilder.OpenWeatherConfig();
        //var clientFactory = ClientBuilder.OpenWeatherClientFactory(OpenWeatherResponses.OkResponse);

        var dbOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("in_memory_db");
        var list = new List<IdentityRole>()
        {
            new IdentityRole("Teacher"),
            new IdentityRole("Student")
        }.AsQueryable();

        var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            new Mock<IRoleStore<IdentityRole>>().Object,
            new IRoleValidator<IdentityRole>[0],
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<ILogger<RoleManager<IdentityRole>>>().Object);

        roleManagerMock.Setup(r => r.Roles).Returns(list);

        using (var db = new ApplicationDbContext(dbOptionsBuilder.Options))
        {
            var fakeUserManager = new Mock<FakeUserManager>();
            


            fakeUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            fakeUserManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync( new List<string>());

            var sut = new AgeTypeServices(db, fakeUserManager.Object);
            var result = await sut.GetUsersAsync();
            var u = await sut.CreateUserAsync();
            result = await sut.GetUsersAsync();

            Assert.Equals(new DateTime(1594155600), new DateTime(1594155600));
        }
    }
}

public class FakeUserManager : UserManager<ApplicationUser>
{
    public FakeUserManager()
        : base(new Mock<IUserStore<ApplicationUser>>().Object,
              new Mock<IOptions<IdentityOptions>>().Object,
              new Mock<IPasswordHasher<ApplicationUser>>().Object,
              new IUserValidator<ApplicationUser>[0],
              new IPasswordValidator<ApplicationUser>[0],
              new Mock<ILookupNormalizer>().Object,
              new Mock<IdentityErrorDescriber>().Object,
              new Mock<IServiceProvider>().Object,
              new Mock<ILogger<UserManager<ApplicationUser>>>().Object)
    { }

}