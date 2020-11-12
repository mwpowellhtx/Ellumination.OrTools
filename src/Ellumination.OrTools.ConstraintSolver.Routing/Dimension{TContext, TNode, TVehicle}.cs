using System;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    // TODO: TBD: ditto basically a placeholder at this moment...
    public abstract class Dimension<TNode, TVehicle, TContext> : Dimension<TContext>
        where TContext : Context<TNode, TVehicle>
    {
        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="context"></param>
        protected Dimension(TContext context)
            : base(context)
        {
        }
    }
}
