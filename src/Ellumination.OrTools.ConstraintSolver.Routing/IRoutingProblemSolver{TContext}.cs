using System;
using System.Collections.Generic;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Provides Routing Model ProblemSolver capability.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IRoutingProblemSolver<TContext> : IDisposable
        where TContext : Context
    {
        /// <summary>
        /// Facilitates the Routing Model problem solution given <paramref name="context"/>.
        /// </summary>
        /// <param name="context"></param>
        void Solve(TContext context);
    }
}
