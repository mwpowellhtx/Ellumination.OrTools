using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Transforms the base <paramref name="assignments"/> in a Domain friendly manner.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="assignments"></param>
        /// <returns></returns>
        private static IEnumerable<(RouteAssignmentItem index, (TVehicle vehicle, TNode node, TNode previousNode) domain)> OnTransformAssignments(
            TContext context, params RouteAssignmentItem[] assignments)
        {
            TNode OnTransformNode(int? node) => context.Nodes.ElementAtOrDefault(node ?? -1);

            (RouteAssignmentItem index
                , (TVehicle vehicle, TNode node, TNode previousNode) domain) OnTransformAssignment(
                    RouteAssignmentItem index) =>
                    (index, (context.Vehicles.ElementAt(index.Vehicle)
                    , OnTransformNode(index.Node), OnTransformNode(index.PreviousNode)
                ));

            foreach (var _ in assignments)
            {
                yield return OnTransformAssignment(_);
            }
        }

        /// <summary>
        /// Gets the Domain oriented Assignments.
        /// </summary>
        public IEnumerable<(RouteAssignmentItem index, (TVehicle vehicle, TNode node, TNode previousNode) domain)> DomainAssignments { get; }

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="context">The Context in use during the currently assigned solution.</param>
        /// <param name="assignments">The Domain oriented assigments during the currently dispatched event.</param>
        internal DomainRoutingAssignmentEventArgs(TContext context, params RouteAssignmentItem[] assignments)
            : base(context, assignments)
        {
            this.DomainAssignments = OnTransformAssignments(context, assignments);
        }
    }
}
