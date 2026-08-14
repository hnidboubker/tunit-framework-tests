using IntApplication.UnitTests.Collections;
using IntCore.Models.Identity;

namespace IntApplication.UnitTests.Helpers
{
    public static class QuerableHelper
    {
        public static IQueryable<User> CreateAsyncQueryable(
           IEnumerable<User> users)
        {
            return new TestAsyncEnumerable<User>(users);
        }
    }
}
