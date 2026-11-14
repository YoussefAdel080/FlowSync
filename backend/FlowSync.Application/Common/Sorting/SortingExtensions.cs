using FlowSync.Contracts.Enums;
using System.Linq.Expressions;

namespace FlowSync.Application.Common.Sorting
{
    public static class SortingExtensions
    {
        public static IOrderedQueryable<T> OrderByField<T, TKey>(
            this IQueryable<T> source,
            Expression<Func<T, TKey>> keySelector,
            SortEnum sort)
        {
            return sort == SortEnum.Ascending
                ? source.OrderBy(keySelector)
                : source.OrderByDescending(keySelector);
        }

        public static IOrderedQueryable<T> ThenByField<T, TKey>(
            this IOrderedQueryable<T> source,
            Expression<Func<T, TKey>> keySelector,
            SortEnum sort)
        {
            return sort == SortEnum.Ascending
                ? source.ThenBy(keySelector)
                : source.ThenByDescending(keySelector);
        }
    }
}