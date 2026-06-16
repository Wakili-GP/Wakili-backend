using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Wakiliy.Application.Tests.Common;

/// <summary>
/// Creates an in-memory IQueryable that supports EF Core async operators
/// (AnyAsync, FirstOrDefaultAsync, Include, Where, etc.) without requiring a real database.
/// </summary>
public static class AsyncQueryableHelper
{
    public static IQueryable<T> BuildAsyncQueryable<T>(this IEnumerable<T> data)
    {
        return new AsyncQueryable<T>(data.AsQueryable());
    }
}

internal class AsyncQueryable<T> : IQueryable<T>, IAsyncEnumerable<T>
{
    private readonly IQueryable<T> _source;

    public AsyncQueryable(IQueryable<T> source)
    {
        _source = source;
        Provider = new AsyncQueryProvider<T>(source.Provider);
        Expression = source.Expression;
    }

    public Type ElementType => typeof(T);
    public Expression Expression { get; }
    public IQueryProvider Provider { get; }

    public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new AsyncEnumerator<T>(_source.GetEnumerator());
}

internal class AsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    public AsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
        => new AsyncQueryable<TEntity>((IQueryable<TEntity>)_inner.CreateQuery<TEntity>(expression));

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new AsyncQueryable<TElement>(_inner.CreateQuery<TElement>(expression));

    public object? Execute(Expression expression)
        => _inner.Execute(expression);

    public TResult Execute<TResult>(Expression expression)
        => _inner.Execute<TResult>(expression);

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        // TResult is Task<T> — extract the inner T
        var resultType = typeof(TResult).GetGenericArguments()[0];

        // Execute synchronously using the in-memory provider
        var syncResult = typeof(IQueryProvider)
            .GetMethods()
            .Single(m => m.Name == nameof(IQueryProvider.Execute) && m.IsGenericMethod)
            .MakeGenericMethod(resultType)
            .Invoke(_inner, new object[] { expression });

        // Wrap in Task.FromResult<T>
        return (TResult)typeof(Task)
            .GetMethod(nameof(Task.FromResult))!
            .MakeGenericMethod(resultType)
            .Invoke(null, new[] { syncResult })!;
    }
}

internal class AsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public AsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current => _inner.Current;

    public ValueTask<bool> MoveNextAsync()
        => ValueTask.FromResult(_inner.MoveNext());

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }
}
