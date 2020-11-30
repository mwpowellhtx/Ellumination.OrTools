using System;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Represents a Domain specific <see cref="RoutingProblemSolver{TContext}"/>.
    /// In which case, we happen to have knowledge are <typeparamref name="TNode"/>,
    /// <typeparamref name="TDepot"/>, and <typeparamref name="TVehicle"/>.
    /// </summary>
    /// <typeparam name="TNode">The Node type.</typeparam>
    /// <typeparam name="TDepot">The Depot type.</typeparam>
    /// <typeparam name="TVehicle">The Vehicle type.</typeparam>
    /// <typeparam name="TContext">The Context type.</typeparam>
    /// <see cref="DomainContext{TNode, TDepot, TVehicle}"/>
    /// <see cref="DomainRoutingAssignmentEventArgs{TNode, TDepot, TVehicle, TContext}"/>
    public abstract class DomainRoutingProblemSolver<TNode, TDepot, TVehicle, TContext>
        : AssignableRoutingProblemSolver<TContext
            , DomainRoutingAssignmentEventArgs<TNode, TDepot, TVehicle, TContext>>
            , IDomainRoutingProblemSolver<TNode, TDepot, TVehicle, TContext
                , DomainRoutingAssignmentEventArgs<TNode, TDepot, TVehicle, TContext>>
        where TDepot : TNode
        where TContext : DomainContext<TNode, TDepot, TVehicle>
    {
        /// <summary>
        /// Returns a Created
        /// <see cref="DomainRoutingAssignmentEventArgs{TNode, TDepot, TVehicle, TContext}"/> instance
        /// given the arguments.
        /// </summary>
        /// <param name="nodeTuple">The <typeparamref name="TNode"/> and Index Tuple.</param>
        /// <param name="vehicleTuple">The <typeparamref name="TVehicle"/> and Index Tuple.</param>
        /// <returns></returns>
        protected virtual DomainRoutingAssignmentEventArgs<TNode, TDepot, TVehicle, TContext> CreateAssignEventArgs(
            TContext context, (int i, TVehicle vehicle) vehicleTuple, (int j, TNode node) nodeTuple) =>
            (DomainRoutingAssignmentEventArgs<TNode, TDepot, TVehicle, TContext>)Activator.CreateInstance(
                typeof(DomainRoutingAssignmentEventArgs<TNode, TDepot, TVehicle, TContext>)
                , context, vehicleTuple, nodeTuple
            );

        /// <inheritdoc/>
        protected override DomainRoutingAssignmentEventArgs<TNode, TDepot, TVehicle, TContext> CreateAssignEventArgs(
            TContext context, int vehicleIndex, int nodeIndex) => CreateAssignEventArgs(context
                , (vehicleIndex, context.Vehicles.ElementAt(vehicleIndex))
                , (nodeIndex, context.Nodes.ElementAt(nodeIndex))
            );
    }
}
