using Microsoft.EntityFrameworkCore;
using Moq;
using skills_hub.core.Helpers;
using skills_hub.core.Repository.LessonType.Interfaces;
using skills_hub.domain.Models.LessonTypes;
using skills_hub.persistence;

namespace skills_hub.tests;

public class AbstractLessonTypeLogModelService : IDisposable
{


    private Mock<ILessonTypeService> _mockLessonTypeService;
    private ApplicationDbContext? _db;
    private IAbstractLogModel<AgeType> _ageTypeService;
    [SetUp]
    public void Setup()
    {
        _mockLessonTypeService = new Mock<ILessonTypeService>(); //new Mock<core.Repository.LessonType.Implementation.LessonTypeService>();
        //_mockLessonTypeService.Setup(x=>x.UpdateAsync())
        _mockLessonTypeService.Setup(x => x.UpdateAsync(It.IsAny<LessonType>(), It.IsAny<Guid[]>()))
            .ReturnsAsync((LessonType item, Guid[] paymentCategories) => new LessonType());

        var dbOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("in_memory_db");
        _db = new ApplicationDbContext(dbOptionsBuilder.Options);
        _ageTypeService = new skills_hub.core.Repository.LessonType.Implementation.AgeTypeService(_db, _mockLessonTypeService.Object);

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
            var items = _ageTypeService.GetCurrentItems().ToList();
            foreach (var item in items)
            {
                await _ageTypeService.RemoveAsync(item.Id);
            }

            var resCreated = await _ageTypeService.CreateAsync(new AgeType()
            {
                MinimumAge = 18,
                MaximumAge = 80,
                Name = "Adults"
            });
            items = _ageTypeService.GetCurrentItems().ToList() ?? new();
            Assert.AreEqual(items.Count(), 1);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [Test]

    public async Task Create_Equals_AgeType_Return_Error()
    {
        var items = _ageTypeService.GetCurrentItems().ToList();
        foreach (var item in items)
        {
            await _ageTypeService.RemoveAsync(item.Id);
        }

        var resCreated = await _ageTypeService.CreateAsync(new AgeType()
        {
            MinimumAge = 18,
            MaximumAge = 80,
            Name = "Adults"
        });
        Assert.That(async () =>
        {
            var res = await _ageTypeService.CreateAsync(new AgeType()
            {
                MinimumAge = 18,
                MaximumAge = 80,
                Name = "Adults2"
            });

        }, Throws.Exception);
    }

    [Test]
    public async Task Update_AgeType()
    {
        var dbOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("in_memory_db");
        var _db = new ApplicationDbContext(dbOptionsBuilder.Options);

        try
        {
            var items = _ageTypeService.GetCurrentItems().ToList();
            foreach (var item in items)
            {
                await _ageTypeService.RemoveAsync(item.Id);
            }
            var resCreated = await _ageTypeService.CreateAsync(new AgeType()
            {
                MinimumAge = 18,
                MaximumAge = 80,
                Name = "Adults"
            });
            items = _ageTypeService.GetCurrentItems().ToList();
            var resUpdated = await _ageTypeService.UpdateAsync(new AgeType()
            {
                Id = resCreated.Id,
                MinimumAge = 18,
                MaximumAge = 90,
                Name = "Adults"
            });
            items = await (await _ageTypeService.GetCurrentItemsWithParentsAsync()).ToListAsync();
            var itemUpdated = items.ToList().FirstOrDefault() ?? new AgeType();
            var allItems = await _ageTypeService.GetItems().ToListAsync();


            Assert.That((itemUpdated.MaximumAge == 90) && (itemUpdated.Parents?.Count() == 1) && allItems.Count == 2);

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
            var items = _ageTypeService.GetCurrentItems().ToList();
            foreach (var item in items)
            {
                await _ageTypeService.RemoveAsync(item.Id);
            }
            var resCreated = await _ageTypeService.CreateAsync(new AgeType()
            {
                MinimumAge = 18,
                MaximumAge = 80,
                Name = "Adults"
            });
            items = _ageTypeService.GetCurrentItems().ToList();
            var resUpdated = await _ageTypeService.RemoveAsync(resCreated.Id);
            var resDeletedItems = await _ageTypeService.GetItems().ToListAsync();

            Assert.That(items.Count == 1 && resDeletedItems.Count == 0);

        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }


}

