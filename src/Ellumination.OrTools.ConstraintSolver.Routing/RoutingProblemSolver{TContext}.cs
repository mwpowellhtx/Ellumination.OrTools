using System;
using System.Collections.Generic;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    // TODO: TBD: from problem solver? to what? "plugin"? "visitor" pattern?
    public abstract class RoutingProblemSolver<TContext> : IRoutingProblemSolver<TContext>
        where TContext : Context
    {
        /// <inheritdoc/>
        public abstract IEnumerable<Dimension<TContext>> Dimensions { get; }

        /// <inheritdoc/>
        public void Solve(TContext context)
        {
        }

        /// <summary>
        /// Gets whether IsDisposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                this.Dispose(true);
            }

            this.IsDisposed = true;
        }
    }
}
