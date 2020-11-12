using System;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using static IncludeEdge;

    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum IncludeEdge
    {
        /// <summary>
        /// 0
        /// </summary>
        IncludeNone = 0,

        /// <summary>
        /// 1 &lt;&lt; 0
        /// </summary>
        IncludeStart = 1 << 0,

        /// <summary>
        /// 1 &lt;&lt; 1
        /// </summary>
        IncludeEnd = 1 << 1
    }

    /// <summary>
    /// Edge extension methods.
    /// </summary>
    internal static class EdgeExtensionMethods
    {
        /// <summary>
        /// Returns the Count around the <paramref name="edge"/> and <paramref name="mask"/>.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static int Summarize(this IncludeEdge edge, IncludeEdge mask) =>
            (edge & mask) == mask ? 1 : 0;

        /// <summary>
        /// Returns the Counts around the <paramref name="edge"/> of the bits.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public static int Summarize(this IncludeEdge edge) =>
            edge.Summarize(IncludeStart) + edge.Summarize(IncludeEnd);
    }
}
