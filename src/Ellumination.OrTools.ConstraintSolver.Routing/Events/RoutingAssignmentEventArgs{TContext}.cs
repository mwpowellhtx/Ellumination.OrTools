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

        // TODO: TBD: similarly with VehicleIndex? or Vehicle?
        /// <summary>
        /// Gets the VehicleIndex.
        /// </summary>
        public int VehicleIndex { get; }

        // TODO: TBD: gets the NodeIndex? or the Node? in RoutingModel terms...
        /// <summary>
        /// Gets the NodeIndex.
        /// </summary>
        public int NodeIndex { get; }

        /// <summary>
        /// Gets the PreviousNodeIndex from which Routing occurred.
        /// </summary>
        public int? PreviousNodeIndex { get; }

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="context">A <typeparamref name="TContext"/> instance.</param>
        /// <param name="vehicleIndex">A Vehicle index.</param>
        /// <param name="nodeIndex">A Node index.</param>
        /// <param name="previousNodeIndex">A Previous Node index from which Routing occurred.</param>
        internal RoutingAssignmentEventArgs(TContext context, int vehicleIndex, int nodeIndex, int? previousNodeIndex)
        {
            this.Context = context;
            this.VehicleIndex = vehicleIndex;
            this.NodeIndex = nodeIndex;
            this.PreviousNodeIndex = previousNodeIndex;
        }
    }
}
