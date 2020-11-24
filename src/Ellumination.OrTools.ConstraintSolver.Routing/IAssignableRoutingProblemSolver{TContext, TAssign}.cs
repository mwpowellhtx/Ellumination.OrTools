using System;
using System.Collections.Generic;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Provides Routing Model ProblemSolver capability.
    /// </summary>
    /// <typeparam name="TContext">The Context type.</typeparam>
    /// <typeparam name="TAssign">The <see cref="Assigning"/>, etc,
    /// <see cref="EventArgs"/>.</typeparam>
    /// <see cref="Context"/>
    /// <see cref="RoutingAssignmentEventArgs{TContext}"/>
    public interface IAssignableRoutingProblemSolver<TContext, TAssign> : IDisposable
        where TContext : Context
        where TAssign : RoutingAssignmentEventArgs<TContext>
    {
        /// <summary>
        /// Event occurs On Solution Assigning.
        /// </summary>
        event EventHandler<TAssign> Assigning;

        /// <summary>
        /// Event occurs On Solution Assign.
        /// </summary>
        event EventHandler<TAssign> Assign;

        /// <summary>
        /// Event occurs On Solution Assigned.
        /// </summary>
        event EventHandler<TAssign> Assigned;

        /// <summary>
        /// Facilitates the Routing Model problem solution given <paramref name="context"/>.
        /// </summary>
        /// <param name="context"></param>
        void Solve(TContext context);
    }
}
