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
using AutoMapper;
using skills_hub.core.Repository.User;
using skills_hub.core.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace skills_hub.tests;

public class AbstractModelTest
{
    private Mock<FakeUserManager> _mockUserManager;
    [SetUp]
    public void Setup()
    {
        _mockUserManager = new Mock<FakeUserManager>(); //IAbstractLogModel<BaseUserInfo>

        _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(x => x.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(new bool());
        _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string>());
    }
        [Test]
    public async Task Create_BaseUserInfo()
    {
        var dbOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("in_memory_db");
        var db = new ApplicationDbContext(dbOptionsBuilder.Options);

        try
        {
            IAbstractLogModel<BaseUserInfo> sut = new BaseUserInfoService(db);
            var items = sut.GetCurrentItems().ToList();
            var resCreated = await sut.CreateAsync(new BaseUserInfo()
            {
                BirthDate = DateTime.Now.AddYears(-35),
                FirstName = "Marina",
                MiddleName = "Dmitrievna",
                Surname = "Malikova"
            });
            items = sut.GetCurrentItems().ToList() ?? new();
            Assert.AreEqual(items.Count(), 1);
        }
        catch (Exception ex) 
        {
           Assert.Fail(ex.Message);
        }
    }

    [Test]
    public async Task Update_BaseUserInfo()//Create_with_not_unique_key_return_error_BaseUserInfo()
    {
        var dbOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("in_memory_db");
        var db = new ApplicationDbContext(dbOptionsBuilder.Options);
        

        try
        {
            IAbstractLogModel<BaseUserInfo> sut = new BaseUserInfoService(db);

            var items = sut.GetCurrentItems().ToList();
            //foreach(var item in items)
            //{
            //    await sut.RemoveAsync(item.Id);
            //}
            var resCreated = await sut.CreateAsync(new BaseUserInfo()
            {
                BirthDate = DateTime.Now.AddYears(-35),
                FirstName = "Marina",
                MiddleName = "Dmitrievna",
                Surname = "Malikova"
            });
            items = sut.GetCurrentItems().ToList();
            var resUpdated = await sut.UpdateAsync(new BaseUserInfo()
            {
                Id = resCreated.Id,
                BirthDate = DateTime.Now.AddYears(-35),
                FirstName = "Marina_updated",
                MiddleName = "Dmitrievna",
                Surname = "Malikova"
            });
            var items2 = await sut.GetCurrentItemsWithParents();
            items = items2.ToList() ?? new();

            Assert.AreEqual(items.Count(), 1);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

}

