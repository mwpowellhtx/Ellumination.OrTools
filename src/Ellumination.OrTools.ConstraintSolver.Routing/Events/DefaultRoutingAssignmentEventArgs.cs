using System;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Provides a set of <see cref="EventArgs"/> for use during the Routing Assignment process.
    /// </summary>
    /// <see cref="RoutingContext"/>
    public class DefaultRoutingAssignmentEventArgs : RoutingAssignmentEventArgs<RoutingContext>
    {
        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="context">A <see cref="RoutingContext"/> instance.</param>
        /// <param name="vehicleIndex">A Vehicle index.</param>
        /// <param name="nodeIndex">A Node index.</param>
        /// <param name="previousNodeIndex">The Previous Node index from which Routing occurred.</param>
        internal DefaultRoutingAssignmentEventArgs(RoutingContext context, int vehicleIndex, int nodeIndex, int? previousNodeIndex)
            : base(context, vehicleIndex, nodeIndex, previousNodeIndex)
        {
        }
    }
}
