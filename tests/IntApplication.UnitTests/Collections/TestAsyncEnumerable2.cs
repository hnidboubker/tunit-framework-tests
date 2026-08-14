//using Microsoft.EntityFrameworkCore.Query;
//using System.Linq.Expressions;
//namespace IntApplication.UnitTests.Collections
//{


//    public class TestAsyncEnumerable<T>
//        : EnumerableQuery<T>,
//          IAsyncEnumerable<T>,
//          IAsyncQueryProvider
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

//        public object? Execute(
//            Expression expression)
//        {
//            return ((IQueryProvider)this).Execute(expression);
//        }

//        public TResult Execute<TResult>(
//            Expression expression)
//        {
//            return ((IQueryProvider)this).Execute<TResult>(expression);
//        }

//        public TResult ExecuteAsync<TResult>(
//            Expression expression,
//            CancellationToken cancellationToken = default)
//        {
//            var expectedResultType =
//                typeof(TResult).GetGenericArguments()[0];

//            var executionResult = typeof(IQueryProvider)
//                .GetMethod(nameof(IQueryProvider.Execute))
//                ?.MakeGenericMethod(expectedResultType)
//                .Invoke(this, new[] { expression });

//            return (TResult)typeof(Task)
//                .GetMethod(nameof(Task.FromResult))!
//                .MakeGenericMethod(expectedResultType)
//                .Invoke(null, new[] { executionResult })!;
//        }

//        public TResult ExecuteAsync<TResult>(
//            Expression expression,
//            CancellationToken cancellationToken = default)
//        {
//            return Task.FromResult(
//                ((IQueryProvider)this).Execute<TResult>(expression));
//        }
//    }
//}
