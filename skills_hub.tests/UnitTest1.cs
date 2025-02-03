using Microsoft.EntityFrameworkCore;
using Moq;
using skills_hub.persistence;

namespace skills_hub.tests;

public class Tests
{
    [Test]
    public async Task TestExampleMethod()
    {
        var dbOptionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase();

        // using Moq as the mocking library
        var utilityServiceMock = new Mock<UtilityService>();
        utilityServiceMock.Setup(u => u.GetRandomNumber()).Returns(4);

        // arrange
        using (var db = new ApplicationDbContext(dbOptionsBuilder.Options))
        {
            // fix up some data
            db.Set<Customer>().Add(new Customer()
            {
                Id = 2,
                Name = "Foo bar"
            });
            await db.SaveChangesAsync();
        }

        using (var db = new MyDbContext(dbOptionsBuilder.Options))
        {
            // create the service
            var service = new ExampleService(logger, db, utilityServiceMock.Object);

            // act
            var result = service.DoSomethingWithCustomer(2);

            // assert
            Assert.NotNull(result);
            Assert.Equal(2, result.CustomerId);
            Assert.Equal("Foo bar", result.CustomerName);
            Assert.Equal(4, result.SomeRandomNumber);
        }
    }
}