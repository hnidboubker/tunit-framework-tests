using IntApplication.UnitTests.Collections;
using IntCore.Models.Identity;
using IntCore.Models.MultiTenancy;

namespace IntApplication.UnitTests.Helpers
{
    public static class QuerableHelper
    {
        public static IQueryable<User> CreateUserAsyncQueryable(
           IEnumerable<User> users)
        {
            return new TestAsyncEnumerable<User>(users);
        } 
        
        public static IQueryable<Role> CreateRoleAsyncQueryable(
           IEnumerable<Role> roles)
        {
            return new TestAsyncEnumerable<Role>(roles);
        } 
        public static IQueryable<Tenant> CreateTenantAsyncQueryable(
           IEnumerable<Tenant> tenants)
        {
            return new TestAsyncEnumerable<Tenant>(tenants);
        }
    }
}
