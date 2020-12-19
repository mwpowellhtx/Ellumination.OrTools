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

                // This works, and perhaps saves us from having to trap a trailing exception when not found.
                return readOnlyList.Select<T, (T item, int i)?>((x, i) => (item: x, i))
                    .FirstOrDefault(z => comparer.Equals(z.Value.item, item))?.i ?? notFound;
            }
            catch
            {
                return notFound;
            }
        }

        /// <summary>
        /// Applies a specified function to the corresponding elements of two sequences,
        /// producing a sequence of the results.
        /// </summary>
        /// <typeparam name="TFirst">The type of the elements of the <paramref name="first"/> input sequence.</typeparam>
        /// <typeparam name="TSecond">The type of the elements of the <paramref name="second"/> input sequence.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the result sequence.</typeparam>
        /// <param name="first">The first sequence to merge.</param>
        /// <param name="second">The second sequence to merge.</param>
        /// <param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains merged elements of two input sequences.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is <c>null</c></exception>
        /// <remarks>This extension method version differs slightly from the base dotnet one in that we relay an index to the
        /// <paramref name="resultSelector"/>, not unlike the Select version which does a similar thing.</remarks>
        /// <see cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, int, TResult})"/>
        /// <see cref="Enumerable.Zip{TFirst, TSecond, TResult}(IEnumerable{TFirst}, IEnumerable{TSecond}, Func{TFirst, TSecond, TResult})"/>
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first
            , IEnumerable<TSecond> second, Func<TFirst, TSecond, int, TResult> resultSelector) =>
            first.Zip(second, (x, y) => (x, y)).Select((z, i) => resultSelector(z.x, z.y, i));
    }
}
