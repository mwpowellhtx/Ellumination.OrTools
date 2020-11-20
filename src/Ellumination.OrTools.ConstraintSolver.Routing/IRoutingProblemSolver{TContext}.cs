using System;
using System.Collections.Generic;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IRoutingProblemSolver<TContext> : IDisposable
        where TContext : Context
    {
        /// <summary>
        /// Gets the Dimensions associated with the Routing ProblemSolver.
        /// </summary>
        IEnumerable<Dimension<TContext>> Dimensions { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        void Solve(TContext context);
    }
}
