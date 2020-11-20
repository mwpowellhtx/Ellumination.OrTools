using System;
using System.Collections.Generic;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TVehicle"></typeparam>
    public class RoutingAssignmentEventArgs<TNode, TVehicle> : EventArgs
    {
        /// <summary>
        /// Gets the Node.
        /// </summary>
        public TNode Node { get; }

        /// <summary>
        /// Gets the Vehicle.
        /// </summary>
        public TVehicle Vehicle { get; }

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="vehicle"></param>
        internal RoutingAssignmentEventArgs(TNode node, TVehicle vehicle)
        {
            this.Node = node;
            this.Vehicle = vehicle;
        }
    }
}
