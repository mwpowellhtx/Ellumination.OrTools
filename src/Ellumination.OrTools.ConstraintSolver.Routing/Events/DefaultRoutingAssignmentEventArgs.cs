using System;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Provides a set of <see cref="EventArgs"/> for use during the Routing Assignment process.
    /// </summary>
    /// <see cref="Routing.Context"/>
    public class DefaultRoutingAssignmentEventArgs : RoutingAssignmentEventArgs<Context>
    {
        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="context">A <see cref="Context"/> instance.</param>
        /// <param name="vehicleIndex">A Vehicle index.</param>
        /// <param name="nodeIndex">A Node index.</param>
        internal DefaultRoutingAssignmentEventArgs(Context context, int vehicleIndex, int nodeIndex)
            : base(context, vehicleIndex, nodeIndex)
        {
        }
    }
}
