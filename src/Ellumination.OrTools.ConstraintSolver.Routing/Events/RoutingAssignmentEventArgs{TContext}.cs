using System;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Provides a set of <see cref="EventArgs"/> for use during the Routing Assignment process.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <see cref="RoutingContext"/>
    public class RoutingAssignmentEventArgs<TContext> : EventArgs
        where TContext : RoutingContext
    {
        /// <summary>
        /// Gets the Context.
        /// </summary>
        public TContext Context { get; }

        /// <summary>
        /// Gets the VehicleIndex.
        /// </summary>
        public int VehicleIndex { get; }

        /// <summary>
        /// Gets the NodeIndex.
        /// </summary>
        public int NodeIndex { get; }

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="context">A <typeparamref name="TContext"/> instance.</param>
        /// <param name="vehicleIndex">A Vehicle index.</param>
        /// <param name="nodeIndex">A Node index.</param>
        internal RoutingAssignmentEventArgs(TContext context, int vehicleIndex, int nodeIndex)
        {
            this.Context = context;
            this.VehicleIndex = vehicleIndex;
            this.NodeIndex = nodeIndex;
        }
    }
}
