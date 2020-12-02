using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
