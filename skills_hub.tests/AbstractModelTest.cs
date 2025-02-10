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

public class AbstractModelTest : IDisposable
{
    private ApplicationDbContext? _db;
    private IAbstractLogModel<BaseUserInfo> _baseUserInfoService;
    [SetUp]
    public void Setup()
    {


        var dbOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("in_memory_db");
        _db = new ApplicationDbContext(dbOptionsBuilder.Options);
        _baseUserInfoService = new BaseUserInfoService(_db);

    }

    public void Dispose()
    {
        _db.Dispose();
    }
    [Test]
    public async Task Create_BaseUserInfo()
    {
        try
        {

            
            var items = _baseUserInfoService.GetCurrentItems().ToList();
            foreach (var item in items)
            {
                await _baseUserInfoService.RemoveAsync(item.Id);
            }

            var resCreated = await _baseUserInfoService.CreateAsync(new BaseUserInfo()
            {
                BirthDate = DateTime.Now.AddYears(-35),
                FirstName = "Marina",
                MiddleName = "Dmitrievna",
                Surname = "Malikova"
            });
            items = _baseUserInfoService.GetCurrentItems().ToList() ?? new();
            Assert.AreEqual(items.Count(), 1);
        }
        catch (Exception ex) 
        {
           Assert.Fail(ex.Message);
        }
    }

    [Test]

    public async Task Create_Equals_BaseUserInfo_Return_Error()
    {
            var items = _baseUserInfoService.GetCurrentItems().ToList();
            foreach (var item in items)
            {
                await _baseUserInfoService.RemoveAsync(item.Id);
            }
            var resCreated = await _baseUserInfoService.CreateAsync(new BaseUserInfo()
            {
                BirthDate = DateTime.Now.AddYears(-35),
                FirstName = "Marina",
                MiddleName = "Dmitrievna",
                Surname = "Malikova"
            });
            Assert.That(async () =>
            {
                var res = await _baseUserInfoService.CreateAsync(new BaseUserInfo()
                {
                    BirthDate = DateTime.Now.AddYears(-35),
                    FirstName = "Marina",
                    MiddleName = "Dmitrievna",
                    Surname = "Malikova"
                });
                
            }, Throws.Exception);
    }

    [Test]
    public async Task Update_BaseUserInfo()//Create_with_not_unique_key_return_error_BaseUserInfo()
    {     
        try
        {
            var items = _baseUserInfoService.GetCurrentItems().ToList();
            foreach (var item in items)
            {
                await _baseUserInfoService.RemoveAsync(item.Id);
            }
            var resCreated = await _baseUserInfoService.CreateAsync(new BaseUserInfo()
            {
                BirthDate = DateTime.Now.AddYears(-35),
                FirstName = "Marina",
                MiddleName = "Dmitrievna",
                Surname = "Malikova"
            });
            items = _baseUserInfoService.GetCurrentItems().ToList();
            var resUpdated = await _baseUserInfoService.UpdateAsync(new BaseUserInfo()
            {
                Id = resCreated.Id,
                BirthDate = DateTime.Now.AddYears(-35),
                FirstName = "Marina_updated",
                MiddleName = "Dmitrievna",
                Surname = "Malikova"
            });
            items = await (await _baseUserInfoService.GetCurrentItemsWithParents()).ToListAsync();
            var itemUpdated = items.ToList().FirstOrDefault() ?? new BaseUserInfo();
            var allItems = await _baseUserInfoService.GetItems().ToListAsync();
            
            
            Assert.That((itemUpdated.FirstName == "Marina_updated") && (itemUpdated.Parents?.Count() == 1) && allItems.Count == 2);

        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [Test]
    public async Task Delete_BaseUserInfo()
    {
        try
        {
            var items = _baseUserInfoService.GetCurrentItems().ToList();
            foreach (var item in items)
            {
                await _baseUserInfoService.RemoveAsync(item.Id);
            }
            var resCreated = await _baseUserInfoService.CreateAsync(new BaseUserInfo()
            {
                BirthDate = DateTime.Now.AddYears(-35),
                FirstName = "Marina",
                MiddleName = "Dmitrievna",
                Surname = "Malikova"
            });
            items = _baseUserInfoService.GetCurrentItems().ToList();
            var resUpdated = await _baseUserInfoService.RemoveAsync(resCreated.Id);
            var resDeletedItems = await _baseUserInfoService.GetItems().ToListAsync();

            Assert.That(items.Count == 1 && resDeletedItems.Count == 0);

        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

}

