using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Ellumination.OrTools
{
    /// <summary>
    /// Provides a set of helpful OrTools Collection extension methods.
    /// </summary>
    internal static class CollectionExtensionMethods
    {
        /// <summary>
        /// Returns the <paramref name="values"/> when Not Null, or an
        /// <see cref="Array.Empty{T}"/> instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        /// <remarks>This is a useful extension method for internal use, but it is not worth
        /// exposing publicly. Let the consumer decide what other extensions to involve in
        /// his or her mix.</remarks>
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> values) => values ?? Array.Empty<T>();

        /// <summary>
        /// Returns a read-only <see cref="ReadOnlyCollection{T}"/> wrapper for the
        /// current collection. Transforms the <paramref name="values"/> first using
        /// <see cref="Enumerable.ToList{TSource}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns>An object that acts as a read-only wrapper around the current
        /// <see cref="IEnumerable{T}"/>.</returns>
        public static IReadOnlyList<T> AsReadOnly<T>(this IEnumerable<T> values) => values.ToList().AsReadOnly();

        /// <summary>
        /// Returns the IndexOf the <paramref name="item"/> in the
        /// <paramref name="readOnlyList"/>. Uses the <see cref="EqualityComparer{T}.Default"/>
        /// instance for comparison purposes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="readOnlyList"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this IReadOnlyList<T> readOnlyList, T item)
        {
            const int notFound = -1;
            try
            {
                var comparer = EqualityComparer<T>.Default;
                return readOnlyList.Select((x, i) => (item: x, i)).First(z => comparer.Equals(z.item, item)).i;
            }
            catch
            {
                return notFound;
            }
        }
    }
}
