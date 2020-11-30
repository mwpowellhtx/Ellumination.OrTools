using System;
using System.Collections.Generic;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Provides a set of <see cref="EventArgs"/> for use during the Routing Assignment process.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TDepot"></typeparam>
    /// <typeparam name="TVehicle"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <see cref="DomainContext{TNode, TDepot, TVehicle}"/>
    /// <see cref="RoutingAssignmentEventArgs{TContext}"/>
    public class DomainRoutingAssignmentEventArgs<TNode, TDepot, TVehicle, TContext> : RoutingAssignmentEventArgs<TContext>
        where TDepot : TNode
        where TContext : DomainContext<TNode, TDepot, TVehicle>
    {
        /// <summary>
        /// Gets the Vehicle.
        /// </summary>
        public TVehicle Vehicle { get; }

        /// <summary>
        /// Gets the Node.
        /// </summary>
        public TNode Node { get; }

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vehicleTuple"></param>
        /// <param name="nodeTuple"></param>
        internal DomainRoutingAssignmentEventArgs(TContext context
            , (int i, TVehicle vehicle) vehicleTuple
            , (int j, TNode node) nodeTuple
        ) : base(context, vehicleTuple.i, nodeTuple.j)
        {
            this.Vehicle = vehicleTuple.vehicle;
            this.Node = nodeTuple.node;
        }
    }
}
