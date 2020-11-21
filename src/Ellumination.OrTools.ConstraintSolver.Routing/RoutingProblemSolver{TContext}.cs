using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Assignment = Google.OrTools.ConstraintSolver.Assignment;
    using RoutingSearchParameters = Google.OrTools.ConstraintSolver.RoutingSearchParameters;

    // TODO: TBD: from problem solver? to what? "plugin"? "visitor" pattern?
    /// <summary>
    /// The idea here is that you derive your <typeparamref name="TContext"/> according to your
    /// modeling needs. You similarly derive a <see cref="RoutingProblemSolver{TContext}"/> along
    /// the same lines, and provide the hooks for the <see cref="Dimension"/> implementations that
    /// apply to your model. This is where you will
    /// <see cref="ContextExtensionMethods.AddDimension{TContext, TDimension}"/> each of those
    /// Dimensions. You may similarly apply any visitors that might otherwise mutate your Context
    /// prior to running the solution to assignment.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class RoutingProblemSolver<TContext> : IRoutingProblemSolver<TContext>
        where TContext : Context
    {
        /// <summary>
        /// Gets the Visitors which apply for the <see cref="RoutingProblemSolver{TContext}"/>.
        /// Override in order to specify the Visitors that Apply to the
        /// <typeparamref name="TContext"/>. Ordering or other constraints are
        /// completely left to consumer discretion as to how to present those Visitors,
        /// in what order, etc.
        /// </summary>
        protected virtual IEnumerable<IVisitor<TContext>> Visitors
        {
            get
            {
                yield break;
            }
        }

        private static TContext Apply(TContext context, params IVisitor<TContext>[] visitors)
        {
            foreach (var visitor in visitors)
            {
                context = visitor.Apply(context);
            }

            // TODO: TBD: apply visitors...
            // TODO: TBD: these have the potential to mutate the base context.
            return context;
        }

        /// <summary>
        /// Applies any Visitors to the <paramref name="context"/> prior to
        /// <see cref="AddDimensions"/>. This is critical, since any mutations to the Context
        /// will potentially disrupt the Dimension, so these must be resolved beforehand.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual TContext ApplyVisitors(TContext context) => Apply(context, this.Visitors.ToArray());

        /// <summary>
        /// Adds the Dimensions corresponding with the <paramref name="context"/>.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void AddDimensions(TContext context)
        {
        }

        /// <summary>
        /// Gets the SearchParameters involved during the <see cref="Solve"/> operation.
        /// </summary>
        protected virtual RoutingSearchParameters SearchParameters { get; }

        /// <summary>
        /// Tries to Solve the <paramref name="context"/> <see cref="Context.Model"/>
        /// given <paramref name="searchParameters"/>. Receives the
        /// <paramref name="solution"/> prior to response.
        /// </summary>
        /// <param name="context">The Context whose Model is being Solved.</param>
        /// <param name="searchParameters">The SearchParameters influencing the
        /// solution.</param>
        /// <param name="solution">Receives the Solution on response.</param>
        /// <returns>Whether a <paramref name="solution"/> was properly received.</returns>
        protected virtual bool TrySolve(TContext context, RoutingSearchParameters searchParameters, out Assignment solution)
        {
            this.AddDimensions(context);

            var model = context.Model;

            solution = null;

            try
            {
                solution = searchParameters == null
                    ? model.Solve()
                    : model.SolveWithParameters(searchParameters);
            }
            catch //(Exception ex)
            {
                // TODO: TBD: potentially log any exceptions...
            }

            return solution != null;
        }


        /// <inheritdoc/>
        public virtual void Solve(TContext context)
        {
            if (!this.TrySolve(this.ApplyVisitors(context), this.SearchParameters, out var solution))
            {
                return;
            }

            using (solution)
            {
                // TODO: TBD: this is where we unpack the assignment vis-a-vis solution...
            }
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
