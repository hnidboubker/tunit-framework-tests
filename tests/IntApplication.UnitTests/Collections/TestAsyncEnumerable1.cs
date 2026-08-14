//using Microsoft.EntityFrameworkCore.Query;
//using System.Linq.Expressions;

//namespace IntApplication.UnitTests.Collections
//{
//    public class TestAsyncEnumerable<T>
//     : EnumerableQuery<T>,
//       IAsyncEnumerable<T>,
//       IAsyncQueryProvider
//    {
//        public TestAsyncEnumerable(IEnumerable<T> enumerable)
//            : base(enumerable)
//        {
//        }

//        public TestAsyncEnumerable(Expression expression)
//            : base(expression)
//        {
//        }

//        public IAsyncEnumerator<T> GetAsyncEnumerator(
//            CancellationToken cancellationToken = default)
//        {
//            return new TestAsyncEnumerator<T>(
//                AsEnumerable().GetEnumerator());
//        }

//        public IQueryable CreateQuery(Expression expression)
//        {
//            return new TestAsyncEnumerable<T>(expression);
//        }

//        public IQueryable<TElement> CreateQuery<TElement>(
//            Expression expression)
//        {
//            return new TestAsyncEnumerable<TElement>(expression);
//        }

//        public object Execute(Expression expression)
//        {
//            return ((IQueryProvider)this).Execute(expression)!;
//        }

//        public TResult Execute<TResult>(Expression expression)
//        {
//            return ((IQueryProvider)this).Execute<TResult>(expression);
//        }

//        public TResult ExecuteAsync<TResult>(
//            Expression expression,
//            CancellationToken cancellationToken = default)
//        {
//            var result = ((IQueryProvider)this)
//                .Execute<TResult>(expression);

//            return Task.FromResult(result);
//        }
//    }

//    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
//    {
//        private readonly IEnumerator<T> _inner;

//        public TestAsyncEnumerator(IEnumerator<T> inner)
//        {
//            _inner = inner;
//        }

//        public T Current => _inner.Current;

//        public ValueTask<bool> MoveNextAsync()
//        {
//            return new ValueTask<bool>(_inner.MoveNext());
//        }

//        public ValueTask DisposeAsync()
//        {
//            _inner.Dispose();
//            return ValueTask.CompletedTask;
//        }
//    }
//}
