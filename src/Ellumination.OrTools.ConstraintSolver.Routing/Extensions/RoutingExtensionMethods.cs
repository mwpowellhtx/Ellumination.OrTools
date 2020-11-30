using System;
using System.Collections.Generic;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    internal static class RoutingExtensionMethods
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
        /// Decouples the Endpoint <paramref name="ep"/>.
        /// </summary>
        /// <param name="ep"></param>
        /// <returns></returns>
        public static IEnumerable<int> DecoupleEndpoint(this (int start, int end) ep)
        {
            yield return ep.start;
            yield return ep.end;
        }
    }
}
