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
    /// <typeparam name="TAssign"></typeparam>
    /// <see cref="Context"/>
    /// <see cref="RoutingAssignmentEventArgs{TContext}"/>
    public abstract class AssignableRoutingProblemSolver<TContext, TAssign>
        : IAssignableRoutingProblemSolver<TContext, TAssign>
        where TContext : Context
        where TAssign : RoutingAssignmentEventArgs<TContext>
    {
        /// <summary>
        /// Gets the Visitors which apply for the
        /// <see cref="AssignableRoutingProblemSolver{TContext, TAssign}"/>. Override in order
        /// to specify the Visitors that Apply to the <typeparamref name="TContext"/>. Ordering
        /// or other constraints are completely left to consumer discretion as to how to present
        /// those Visitors, in what order, etc.
        /// </summary>
        protected virtual IEnumerable<IVisitor<TContext>> Visitors
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Applies the <paramref name="visitor"/> to the <paramref name="context"/>
        /// in the <see cref="Apply"/> Aggregate.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="visitor"></param>
        /// <returns></returns>
        protected virtual TContext OnAggregateVisitor(TContext context, IVisitor<TContext> visitor) => visitor.Apply(context);

        /// <summary>
        /// Applies each of the <paramref name="visitors"/> instances to the
        /// <paramref name="context"/>, allowing for each opportunity to potentially
        /// mutate the Context accordingly.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="visitors"></param>
        /// <returns></returns>
        private TContext Apply(TContext context, params IVisitor<TContext>[] visitors) => visitors.Aggregate(context, this.OnAggregateVisitor);

        /// <summary>
        /// Applies any Visitors to the <paramref name="context"/> prior to
        /// <see cref="AddDimensions"/>. This is critical, since any mutations to the Context
        /// will potentially disrupt the Dimension, so these must be resolved beforehand.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual TContext ApplyVisitors(TContext context) => this.Apply(context, this.Visitors.ToArray());

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
        /// Returns a Created <see cref="RoutingAssignmentEventArgs{TContext}"/> given
        /// <paramref name="context"/>, <paramref name="nodeIndex"/>, and
        /// <paramref name="vehicleIndex"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vehicleIndex"></param>
        /// <param name="nodeIndex"></param>
        /// <returns></returns>
        protected virtual TAssign CreateAssignEventArgs(TContext context, int vehicleIndex, int nodeIndex) =>
            (TAssign)Activator.CreateInstance(typeof(TAssign), context, vehicleIndex, nodeIndex);

        /// <inheritdoc/>
        public virtual event EventHandler<TAssign> Assigning;

        /// <inheritdoc/>
        public virtual event EventHandler<TAssign> Assign;

        /// <inheritdoc/>
        public virtual event EventHandler<TAssign> Assigned;

        /// <summary>
        /// Event dispatcher On <see cref="Assigning"/> of <paramref name="e"/> arguments.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAssigningSolution(TAssign e) => this.Assigning?.Invoke(this, e);

        /// <summary>
        /// Event dispatcher On <see cref="Assign"/> of <paramref name="e"/> arguments.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAssignSolution(TAssign e) => this.Assign?.Invoke(this, e);

        /// <summary>
        /// Event dispatcher On <see cref="Assigned"/> of <paramref name="e"/> arguments.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAssignedSolution(TAssign e) => this.Assigned?.Invoke(this, e);

        /// <summary>
        /// Callback occurs OnProcessSolution given <paramref name="context"/>,
        /// <paramref name="nodeIndex"/>, and <paramref name="vehicleIndex"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vehicleIndex"></param>
        /// <param name="nodeIndex"></param>
        /// <see cref="TryProcessSolution"/>
        protected virtual void OnProcessSolution(TContext context, int vehicleIndex, int nodeIndex)
        {
            var e = this.CreateAssignEventArgs(context, vehicleIndex, nodeIndex);

            this.OnAssigningSolution(e);

            this.OnAssignSolution(e);

            this.OnAssignedSolution(e);
        }

        /// <summary>
        /// Tries to Process the <see cref="Assignment"/> <paramref name="solution"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="solution"></param>
        /// <returns></returns>
        /// <see cref="OnProcessSolution"/>
        protected virtual bool TryProcessSolution(TContext context, Assignment solution)
        {
            var context_Model = context.Model;
            var context_Manager = context.Manager;
            var context_VehicleCount = context.VehicleCount;

            // i: vehicle index, specifically declared int for clarity.
            for (int vehicleIndex = 0; vehicleIndex < context_VehicleCount; vehicleIndex++)
            {
                long j;

                // j: node index, specifically declared long for clarity.
                for (j = context_Model.Start(vehicleIndex); !context_Model.IsEnd(j); j = solution.Value(context_Model.NextVar(j)))
                {
                    this.OnProcessSolution(context, vehicleIndex, context_Manager.IndexToNode(j));
                }

                // TODO: TBD: is this really necessary?
                // TODO: TBD: according to at least one example, it may be necessary...
                // TODO: TBD: https://developers.google.com/optimization/routing/vrp
                this.OnProcessSolution(context, vehicleIndex, context_Manager.IndexToNode(j));
            }

            return true;
        }

        /// <summary>
        /// Tries to Solve the <paramref name="context"/> <see cref="Context.Model"/>
        /// given <paramref name="searchParameters"/>. Receives the
        /// <paramref name="solution"/> prior to response.
        /// </summary>
        /// <param name="context">The Context whose Model is being Solved. Assumes that the
        /// <see cref="Visitors"/> have all been applied and any <see cref="Context"/> mutations
        /// that were going to occur have all occurred. So, should be safe now to add dimensions
        /// to the <see cref="Context.Model"/>.</param>
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

            // Problem solver owns the Solution so therefore we can use it here.
            using (solution)
            {
                if (!this.TryProcessSolution(context, solution))
                {
                    // TODO: TBD: log when processing could not be accomplished...
                }
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
