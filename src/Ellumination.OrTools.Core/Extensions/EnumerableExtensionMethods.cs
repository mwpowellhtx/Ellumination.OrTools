using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
    using Ellumination.OrTools;

    /// <summary>
    /// 
    /// </summary>
    /// <see cref="!:https://docs.microsoft.com/en-us/dotnet/core/tutorials/libraries"/>
    /// <see cref="!:https://stackoverflow.com/questions/38476796/how-to-set-net-core-in-if-statement-for-compilation"/>
    /// <see cref="!:https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.tohashset"/>
    internal static class EnumerableExtensionMethods
    {

        // TODO: TBD: may also need/want .NET 5.0, .NET Core 2.0, and .NET Framework 4.7.2 comprehension...
        // TODO: TBD: however, for now, and for this purpose, this is sufficient.
#if !NETSTANDARD2_1

        /// <summary>
        /// Creates a <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/>
        /// <paramref name="values"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="values"></param>
        /// <returns>A <see cref="HashSet{T}"/> that contains values of type
        /// <typeparamref name="TSource"/> selected from the input sequence.</returns>
        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> values) => new HashSet<TSource>(values.OrEmpty());

        /// <summary>
        /// Creates a <see cref="HashSet{T}"/> from <see cref="IEnumerable{T}"/>
        /// <paramref name="values"/> using the <paramref name="comparer"/> to compare keys.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="values"><see cref="IEnumerable{T}"/> Values from which to create a <see cref="HashSet{T}"/>.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
        /// <returns>A <see cref="HashSet{T}"/> that contains values of type
        /// <typeparamref name="TSource"/> selected from the input sequence.</returns>
        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> values, IEqualityComparer<TSource> comparer) =>
            new HashSet<TSource>(values.OrEmpty(), comparer);

#endif

    }
}
