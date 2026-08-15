using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace IntApplication.UnitTests.Collections
{
    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
            => new TestAsyncEnumerable<TEntity>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(
            Expression expression)
            => new TestAsyncEnumerable<TElement>(expression);

        public object? Execute(Expression expression)
            => _inner.Execute(expression);

        public TResult Execute<TResult>(Expression expression)
            => _inner.Execute<TResult>(new ReplaceAsyncCallsVisitor().Visit(expression));

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var resultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                .GetMethod(nameof(IQueryProvider.Execute), 1, new[] { typeof(Expression) })!
                .MakeGenericMethod(resultType)
                .Invoke(this, new[] { expression });

            return (TResult)typeof(Task)
                .GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(resultType)
                .Invoke(null, new[] { executionResult })!;
        }

        /// <summary>
        /// EF Core async extension methods (FirstOrDefaultAsync, SingleOrDefaultAsync, ...)
        /// cannot be executed by an in-memory EnumerableQuery. Replace them with their
        /// synchronous LINQ-to-Objects equivalents before executing.
        /// </summary>
        private sealed class ReplaceAsyncCallsVisitor : ExpressionVisitor
        {
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                var asyncName = node.Method.Name;
                if (!asyncName.EndsWith("Async"))
                    return base.VisitMethodCall(node);

                var queryableType = node.Method.DeclaringType;
                if (queryableType?.FullName != "Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions")
                    return base.VisitMethodCall(node);

                var syncName = asyncName[..^"Async".Length];
                var source = Visit(node.Arguments[0]);
                var args = node.Arguments.Skip(1).Select(Visit).ToList();

                var elementType = node.Method.GetGenericArguments().First();

                if (syncName == "Any" ||
                    syncName == "All" ||
                    syncName == "Count" ||
                    syncName == "LongCount" ||
                    syncName == "First" ||
                    syncName == "FirstOrDefault" ||
                    syncName == "Last" ||
                    syncName == "LastOrDefault" ||
                    syncName == "Single" ||
                    syncName == "SingleOrDefault")
                {
                    var qMethod = typeof(Queryable)
                        .GetMethods()
                        .Single(m =>
                            m.Name == syncName &&
                            m.GetGenericArguments().Length == 1 &&
                            m.GetParameters().Length == args.Count + 1)
                        .MakeGenericMethod(elementType);

                    return Expression.Call(qMethod, new[] { source }.Concat(args));
                }

                if (syncName == "ToArray")
                {
                    var method = typeof(Enumerable)
                        .GetMethod(nameof(Enumerable.ToArray))!
                        .MakeGenericMethod(elementType);
                    return Expression.Call(method, source);
                }

                if (syncName == "ToList")
                {
                    var method = typeof(Enumerable)
                        .GetMethod(nameof(Enumerable.ToList))!
                        .MakeGenericMethod(elementType);
                    return Expression.Call(method, source);
                }

                return base.VisitMethodCall(node);
            }
        }
    }
}
