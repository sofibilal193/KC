using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace KC.Domain.Common.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IQueryable">IQueryable</see> and <see cref="IOrderedQueryable">IOrderedQueryable</see>.
    /// </summary>
    public static class OrderByExtensions
    {
        #region OrderBy

        /// <summary>
        /// Sorts the elements of a sequence in ascending or descending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="sortDescending">Whether to sort descending.</param>
        /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a key.</returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, bool sortDescending)
        {
            if (sortDescending)
            {
                return source.OrderByDescending(keySelector);
            }
            else
            {
                return source.OrderBy(keySelector);
            }
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending or descending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="comparer">An System.Collections.Generic.IComparer`1 to compare keys.</param>
        /// <param name="sortDescending">Whether to sort descending.</param>
        /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a key.</returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer, bool sortDescending)
        {
            if (sortDescending)
            {
                return source.OrderByDescending(keySelector, comparer);
            }
            else
            {
                return source.OrderBy(keySelector, comparer);
            }
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending or descending order by one or more properties.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="sortExpression">A comma-delimited list of property names to sort by. To sort in descending order, prefix the property with "-".</param>
        /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a key.</returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string sortExpression)
        {
            if (string.IsNullOrWhiteSpace(sortExpression))
            {
                throw new ArgumentException("sortExpression cannot be null or empty.", nameof(sortExpression));
            }
            var properties = sortExpression.Split(',', StringSplitOptions.TrimEntries);
            var methodName = properties[0].StartsWith('-') ? "OrderByDescending" : "OrderBy";
            var result = OrderBy(source, methodName, properties[0].TrimStart('-'));
            foreach (var property in properties.Skip(1))
            {
                methodName = property.StartsWith('-') ? "ThenByDescending" : "ThenBy";
                result = OrderBy(result, methodName, property.TrimStart('-'));
            }
            return result;
        }

        #endregion

        #region ThenBy

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence ine in ascending or descending order according to a property value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="sortDescending">Whether to sort descending.</param>
        /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a key.</returns>
        public static IOrderedQueryable<TSource> ThenBy<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, bool sortDescending)
        {
            if (sortDescending)
            {
                return source.ThenByDescending(keySelector);
            }
            else
            {
                return source.ThenBy(keySelector);
            }
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence ine in ascending or descending order according to a property value.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="comparer">An System.Collections.Generic.IComparer`1 to compare keys.</param>
        /// <param name="sortDescending">Whether to sort descending.</param>
        /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a key.</returns>
        public static IOrderedQueryable<TSource> ThenBy<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer, bool sortDescending)
        {
            if (sortDescending)
            {
                return source.ThenByDescending(keySelector, comparer);
            }
            else
            {
                return source.ThenBy(keySelector, comparer);
            }
        }

        #endregion

        #region Private Methods

        private static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string methodName, string propertyName)
        {
            var type = typeof(TSource);
            var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                ?? throw new ArgumentException($"{type.FullName} does not contain a property named '{propertyName}'", nameof(propertyName));
            var parameterExpression = Expression.Parameter(type, "p");
            var propertyExpression = Expression.Property(parameterExpression, property);
            var lambdaExpression = Expression.Lambda(propertyExpression, parameterExpression);
            var orderByExpression = Expression.Call(typeof(Queryable), methodName, new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(lambdaExpression));
            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(orderByExpression);
        }

        #endregion
    }
}
