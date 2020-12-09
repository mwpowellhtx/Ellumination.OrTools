using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    internal static class RoutingExtensionMethods
    {
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xDim"></param>
        /// <param name="yDim"></param>
        /// <returns></returns>
        public static IEnumerable<(int x, int y)> AsCoordinates(this IEnumerable<int> xDim, IEnumerable<int> yDim) =>
            xDim.SelectMany(x => yDim.Select(y => (x, y)));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        public static IEnumerable<(int x, int y)> AsCoordinates(this IEnumerable<int> dim) => dim.AsCoordinates(dim);
    }
}
