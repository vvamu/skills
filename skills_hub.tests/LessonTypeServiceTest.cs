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
using skills_hub.tests.Helpers;
using skills_hub.core.Repository.LessonType.Interfaces;

namespace skills_hub.tests;

public class LessonTypeServiceTest : IDisposable
{
    private Mock<ILessonTypeService> _mockLessonTypeService;
    private ApplicationDbContext? _db;
    private ILessonTypeService _lessonTypeService;
    [SetUp]
    public void Setup()
    {
        _mockLessonTypeService = new Mock<ILessonTypeService>(); //new Mock<core.Repository.LessonType.Implementation.LessonTypeService>()
        _mockLessonTypeService.Setup(x => x.UpdateAsync(It.IsAny<LessonType>(), It.IsAny<Guid[]>()))
            .ReturnsAsync((LessonType item, Guid[] paymentCategories) =>new LessonType());

        var dbOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("in_memory_db");
        _db = new ApplicationDbContext(dbOptionsBuilder.Options);
        _lessonTypeService = new skills_hub.core.Repository.LessonType.Implementation.LessonTypeService(_db);

    }

    public void Dispose()
    {
        _db.Dispose();
    }
   

    
}

