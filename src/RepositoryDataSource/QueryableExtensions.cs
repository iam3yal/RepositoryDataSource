namespace Aspx.WebControls;

using System.Linq;
using System.Linq.Expressions;
    
internal static class QueryableExtensions
{
    private static IOrderedQueryable<TSource> OrderBy<TSource>(
        IQueryable<TSource> source,
        string orderByMethod,
        string propertyName,
        string direction)
    {
        //Contract.Requires<ArgumentNullException>(source != null);
        //Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(propertyName));

        if (!string.IsNullOrEmpty(direction) && direction.ToLowerInvariant() == "desc")
        {
            orderByMethod = $"{orderByMethod}Descending";
        }

        var parameterExpression = Expression.Parameter(source.ElementType);
        var propertyExpression = Expression.PropertyOrField(parameterExpression, propertyName);
        var selector = Expression.Lambda(propertyExpression, parameterExpression);

        var orderByCallExpression = Expression.Call(
            typeof(Queryable),
            orderByMethod,
            [source.ElementType, propertyExpression.Type],
            source.Expression,
            selector);

        return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(orderByCallExpression);
    }

    public static IOrderedQueryable<TSource> OrderBy<TSource>(
        this IQueryable<TSource> source,
        string propertyName,
        string direction) => OrderBy(source, "OrderBy", propertyName, direction);

    public static IOrderedQueryable<TSource> ThenBy<TSource>(
        this IOrderedQueryable<TSource> source,
        string propertyName,
        string direction) => OrderBy(source, "ThenBy", propertyName, direction);

    public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int page, int size)
        where TSource : class
    {
        //Contract.Requires<ArgumentNullException>(source != null);
        //Contract.Requires<ArgumentOutOfRangeException>(page >= 0);
        //Contract.Requires<ArgumentOutOfRangeException>(size > 0);

        var index = GetIndex(page, size);

        return source.Skip(index).Take(size);
    }

    private static int GetIndex(int page, int size) 
        => page * size;
}