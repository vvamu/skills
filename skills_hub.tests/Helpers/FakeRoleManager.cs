using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace skills_hub.tests.Helpers;

public class FakeRoleManager : RoleManager<IdentityRole<Guid>>
{
    public FakeRoleManager()
        : base(
        new Mock<IRoleStore<IdentityRole<Guid>>>().Object,
        new IRoleValidator<IdentityRole<Guid>>[0],
        new Mock<ILookupNormalizer>().Object,
        new Mock<IdentityErrorDescriber>().Object,
        new Mock<ILogger<RoleManager<IdentityRole<Guid>>>>().Object)
    { }

}
