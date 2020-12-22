using System;
using System.Collections.Generic;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Assignment = Google.OrTools.ConstraintSolver.Assignment;

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
        /// Gets the Assignment associated with the event.
        /// </summary>
        public Assignment Solution { get; }

        /// <summary>
        /// Gets the Assignments in the currently dispatched event.
        /// </summary>
        public IEnumerable<RouteAssignmentItem> Assignments { get; }

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="context">A <typeparamref name="TContext"/> instance.</param>
        /// <param name="solution">The Assignment associated with the event.</param>
        /// <param name="assignments">The Assignments in the currently dispatched event.</param>
        internal RoutingAssignmentEventArgs(TContext context, Assignment solution, params RouteAssignmentItem[] assignments)
        {
            this.Context = context;
            this.Solution = solution;
            this.Assignments = assignments;
        }
    }
}
