using System;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    // TODO: TBD: will consider dimension bits next...
    // TODO: TBD: for now this is simply a placeholder...
    // TODO: TBD: with the intention of cross referencing with the Context in some way shape or form...
    public abstract class Dimension<TContext>
        where TContext : Context
    {
        /// <summary>
        /// Gets the Context associated with the Dimension.
        /// </summary>
        protected TContext Context { get; }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="context"></param>
        protected Dimension(TContext context)
        {
            this.Context = context;
        }
    }
}
