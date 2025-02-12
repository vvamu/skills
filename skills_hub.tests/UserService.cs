using skills_hub.persistence;
using skills_hub.tests.Helpers;

namespace skills_hub.tests
{
    internal class UserService
    {
        private ApplicationDbContext? db;
        private FakeUserManager object1;
        private FakeRoleManager object2;

        public UserService(ApplicationDbContext? db, FakeUserManager object1, FakeRoleManager object2)
        {
            this.db = db;
            this.object1 = object1;
            this.object2 = object2;
        }
    }
}