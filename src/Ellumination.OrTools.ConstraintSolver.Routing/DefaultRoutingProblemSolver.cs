using System;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Represents a Default <see cref="AssignableRoutingProblemSolver{TContext, TAssign}"/>.
    /// </summary>
    /// <see cref="RoutingContext"/>
    /// <see cref="RoutingAssignmentEventArgs{TContext}"/>
    public class DefaultRoutingProblemSolver
        : AssignableRoutingProblemSolver<RoutingContext, DefaultRoutingAssignmentEventArgs>
            , IAssignableRoutingProblemSolver<RoutingContext, DefaultRoutingAssignmentEventArgs>
    {
        /// <inheritdoc/>
        protected override DefaultRoutingAssignmentEventArgs CreateAssignEventArgs(
            RoutingContext context, int vehicleIndex, int nodeIndex, int? previousNodeIndex) =>
            new DefaultRoutingAssignmentEventArgs(context, vehicleIndex, nodeIndex, previousNodeIndex);
    }
}
