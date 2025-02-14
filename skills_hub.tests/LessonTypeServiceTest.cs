using Microsoft.EntityFrameworkCore;
using Moq;
using skills_hub.core.Helpers;
using skills_hub.core.Repository.LessonType.Implementation;
using skills_hub.core.Repository.LessonType.Interfaces;
using skills_hub.core.Repository.User;
using skills_hub.domain.Models.LessonTypes;
using skills_hub.persistence;

namespace skills_hub.tests;

public class LessonTypeServiceTest : IDisposable
{
    private ApplicationDbContext? _db;
    private ILessonTypeService _lessonTypeService;
    private IAbstractLogModel<PaymentCategory> _paymentCategoryService;
    private Mock<IAbstractLogModel<PaymentCategory>> _mockPaymentCategoryService;

    [SetUp]
    public void Setup()
    {
        var dbOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("in_memory_db");
        _db = new ApplicationDbContext(dbOptionsBuilder.Options);
        _lessonTypeService = new skills_hub.core.Repository.LessonType.Implementation.LessonTypeService(_db);
        //_mockPaymentCategoryService = new Mock<IAbstractLogModel<PaymentCategory>>(_db, _lessonTypeService);
        _paymentCategoryService = new PaymentCategoryService(_db, _lessonTypeService);

    }

    public void Dispose()
    {
        _db.Dispose();
    }

    [Test]
    public async Task Create_Without_Payment_Categories_Returns_Error()
    {
        try
        {
            var items = _lessonTypeService.GetCurrentItems().ToList();
            foreach (var item in items)
            {
                await _lessonTypeService.RemoveAsync(item.Id);
            }

            Assert.That(async () =>
            {
                var res = await _lessonTypeService.CreateAsync(new LessonType()
                {
                    Name = "Default",
                    LessonTimeInMinutes = 10,
                    DurationTypeName = "lesson",
                    DurationTypeValue = 10,
                    FrequencyName = "week",
                    FrequencyValue = 2
                }, new Guid[1]);
            }, Throws.Exception);

        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [Test]
    public async Task Create()
    {
        try
        {
            var itemsPayement = _paymentCategoryService.GetCurrentItems().ToList();
            foreach (var item in itemsPayement)
            {
                await _paymentCategoryService.RemoveAsync(item.Id);
            }
            itemsPayement = _paymentCategoryService.GetCurrentItems().ToList();

            var paymentCategory = new PaymentCategory()
            {
                StudentPrice = 10,
                TeacherPrice = 10,
                MinCountLessonsToPay = 1,
                DurationTypeTeacherName = "lesson",
                DurationTypeTeacherValue = 1,
                DurationTypeStudentName = "lesson",
                DurationTypeStudentValue = 1,
            };
            await _paymentCategoryService.CreateAsync(paymentCategory);
            var paymentCategories = _paymentCategoryService.GetItems().ToList();

            var items = _lessonTypeService.GetCurrentItems().ToList();
            foreach (var item in items)
            {
                await _lessonTypeService.RemoveAsync(item.Id);
            }

            Assert.That(async () =>
            {
                var res = await _lessonTypeService.CreateAsync(new LessonType()
                {
                    Name = "Default",
                    LessonTimeInMinutes = 10,
                    DurationTypeName = "lesson",
                    DurationTypeValue = 10,
                    FrequencyName = "week",
                    FrequencyValue = 2
                }, new Guid[1] { paymentCategories.FirstOrDefault().Id });
            }, Throws.Exception);

        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

}

