using System;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Assignment = Google.OrTools.ConstraintSolver.Assignment;

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
        /// <param name="solution">The Assignment associated with the current event.</param>
        /// <param name="assignments">The Assignments in the currently dispatched event.</param>
        internal DefaultRoutingAssignmentEventArgs(RoutingContext context, Assignment solution, params RouteAssignmentItem[] assignments)
            : base(context, solution, assignments)
        {
        }
    }
}
