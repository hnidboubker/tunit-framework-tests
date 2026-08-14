using Moq;

namespace IntApplication.UnitTests.Managers
{
    public sealed class MockManager
    {
        private readonly Dictionary<Type, object> _mocks = new();

        public Mock<T> Get<T>()
            where T : class
        {
            if (_mocks.TryGetValue(typeof(T), out var mock))
                return (Mock<T>)mock;

            var created = new Mock<T>();

            _mocks.Add(typeof(T), created);

            return created;
        }
    }
}
