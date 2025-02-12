using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using skills_hub.core.Repository.User.Interfaces;
using skills_hub.domain.Models.User;
using skills_hub.persistence;
using skills_hub.tests.Helpers;

namespace skills_hub.tests;

public class NotificationServiceTest : IDisposable
{

    private INotificationService _notificationService;
    private Mock<FakeUserManager> _mockUserManager;
    private ApplicationDbContext? _db;

    [SetUp]
    public void Setup()
    {
        _mockUserManager = new Mock<FakeUserManager>(); //IAbstractLogModel<BaseUserInfo>
        _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(x => x.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(new bool());
        _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string>());

        var dbOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("in_memory_db");
        _db = new ApplicationDbContext(dbOptionsBuilder.Options);
        _notificationService = new core.Repository.User.NotificationService(_db, (UserManager<ApplicationUser>)_mockUserManager.Object);

    }

    public void Dispose()
    {
        _db.Dispose();
    }
    [Test]
    public async Task Create_AgeType()
    {
        try
        {

            //var items = _notificationService.GetCurrentItems().ToList();
            //foreach (var item in items)
            //{
            //    await _notificationService.RemoveAsync(item.Id);
            //}

            //var resCreated = await _notificationService.CreateAsync(new AgeType()
            //{
            //    MinimumAge = 18,
            //    MaximumAge = 80,
            //    Name = "Adults"
            //});
            //items = _notificationService.GetCurrentItems().ToList() ?? new();
            //Assert.AreEqual(items.Count(), 1);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [Test]

    public async Task Create_Equals_AgeType_Return_Error()
    {
        //var items = _notificationService.GetCurrentItems().ToList();
        //foreach (var item in items)
        //{
        //    await _notificationService.RemoveAsync(item.Id);
        //}

        //var resCreated = await _notificationService.CreateAsync(new AgeType()
        //{
        //    MinimumAge = 18,
        //    MaximumAge = 80,
        //    Name = "Adults"
        //});
        //Assert.That(async () =>
        //{
        //    var res = await _notificationService.CreateAsync(new AgeType()
        //    {
        //        MinimumAge = 18,
        //        MaximumAge = 80,
        //        Name = "Adults2"
        //    });

        //}, Throws.Exception);
    }

    [Test]
    public async Task Update_AgeType()
    {
        try
        {
            //var items = _notificationService.GetCurrentItems().ToList();
            //foreach (var item in items)
            //{
            //    await _notificationService.RemoveAsync(item.Id);
            //}
            //var resCreated = await _notificationService.CreateAsync(new AgeType()
            //{
            //    MinimumAge = 18,
            //    MaximumAge = 80,
            //    Name = "Adults"
            //});
            //items = _notificationService.GetCurrentItems().ToList();
            //var resUpdated = await _notificationService.UpdateAsync(new AgeType()
            //{
            //    Id = resCreated.Id,
            //    MinimumAge = 18,
            //    MaximumAge = 90,
            //    Name = "Adults"
            //});
            //items = await (await _notificationService.GetCurrentItemsWithParents()).ToListAsync();
            //var itemUpdated = items.ToList().FirstOrDefault() ?? new AgeType();
            //var allItems = await _notificationService.GetItems().ToListAsync();


            //Assert.That((itemUpdated.MaximumAge == 90) && (itemUpdated.Parents?.Count() == 1) && allItems.Count == 2);

        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [Test]
    public async Task Delete_AgeType()
    {
        try
        {
            //var items = _notificationService.GetCurrentItems().ToList();
            //foreach (var item in items)
            //{
            //    await _notificationService.RemoveAsync(item.Id);
            //}
            //var resCreated = await _notificationService.CreateAsync(new AgeType()
            //{
            //    MinimumAge = 18,
            //    MaximumAge = 80,
            //    Name = "Adults"
            //});
            //items = _notificationService.GetCurrentItems().ToList();
            //var resUpdated = await _notificationService.RemoveAsync(resCreated.Id);
            //var resDeletedItems = await _notificationService.GetItems().ToListAsync();

            //Assert.That(items.Count == 1 && resDeletedItems.Count == 0);

        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }


}



