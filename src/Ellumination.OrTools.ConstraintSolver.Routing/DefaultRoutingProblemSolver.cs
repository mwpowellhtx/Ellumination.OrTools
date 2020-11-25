using System;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Represents a Default <see cref="AssignableRoutingProblemSolver{TContext, TAssign}"/>.
    /// </summary>
    /// <see cref="Context"/>
    /// <see cref="RoutingAssignmentEventArgs{TContext}"/>
    public abstract class DefaultRoutingProblemSolver
        : AssignableRoutingProblemSolver<Context, DefaultRoutingAssignmentEventArgs>
            , IAssignableRoutingProblemSolver<Context, DefaultRoutingAssignmentEventArgs>
    {
        /// <inheritdoc/>
        protected override DefaultRoutingAssignmentEventArgs CreateAssignEventArgs(
            Context context, int vehicleIndex, int nodeIndex) =>
            new DefaultRoutingAssignmentEventArgs(context, vehicleIndex, nodeIndex);
    }
}
