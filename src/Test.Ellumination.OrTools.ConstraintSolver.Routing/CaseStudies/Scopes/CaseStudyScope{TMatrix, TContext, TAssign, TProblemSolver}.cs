using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using static TestFixtureBase;

    /// <summary>
    /// Specifies certain bits that are common throughout the Case Studies.
    /// </summary>
    /// <typeparam name="TMatrix"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TAssign"></typeparam>
    /// <typeparam name="TProblemSolver"></typeparam>
    public abstract class CaseStudyScope<TMatrix, TContext, TAssign, TProblemSolver> : CaseStudyScope
        where TMatrix : Distances.Matrix
        where TContext : RoutingContext
        where TAssign : RoutingAssignmentEventArgs<TContext>
        where TProblemSolver : AssignableRoutingProblemSolver<TContext, TAssign>
    {
        private TMatrix _matrix;
        private TContext _context;
        private TProblemSolver _problemSolver;

        /// <summary>
        /// Returns a Created <typeparamref name="TMatrix"/> instance.
        /// </summary>
        /// <returns></returns>
        /// <see cref="Matrix"/>
        protected abstract TMatrix CreateMatrix();

        /// <summary>
        /// Returns a Created <typeparamref name="TContext"/> instance.
        /// </summary>
        /// <returns></returns>
        /// <see cref="Context"/>
        protected abstract TContext CreateContext();

        /// <summary>
        /// Returns a new <typeparamref name="TProblemSolver"/> instance.
        /// </summary>
        /// <returns></returns>
        /// <see cref="ProblemSolver"/>
        protected virtual TProblemSolver CreateProblemSolver()
        {
            var problemSolver = Activator.CreateInstance<TProblemSolver>();
            problemSolver.Assign += this.OnProblemSolverAssign;
            return problemSolver;
        }

        /// <summary>
        /// Gets the Matrix associated with the Case Study.
        /// </summary>
        /// <see cref="CreateMatrix"/>
        internal virtual TMatrix Matrix => this._matrix ?? (this._matrix = this.CreateMatrix());

        /// <summary>
        /// Gets the Context associated with the Case Study.
        /// </summary>
        /// <see cref="CreateContext"/>
        internal virtual TContext Context => this._context ?? (this._context = this.CreateContext());

        /// <summary>
        /// Gets or Sets the ProblemSolver.
        /// </summary>
        /// <see cref="CreateProblemSolver"/>
        internal TProblemSolver ProblemSolver => this._problemSolver ?? (this._problemSolver = CreateProblemSolver());

        /// <inheritdoc/>
        protected CaseStudyScope(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Event handler occurs On
        /// <see cref="AssignableRoutingProblemSolver{TContext, TAssign}.Assign"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnProblemSolverAssign(object sender, TAssign e)
        {
            var (vehicle, node, previousNode) = (e.AssertNotNull().VehicleIndex, e.NodeIndex, e.PreviousNodeIndex);

            // Should always have a SolutionPath corresponding to the Vehicle.
            this.SolutionPaths.ElementAt(vehicle).AssertNotNull().Add(node);

            if (previousNode.HasValue)
            {
                var transitCost = this.Context.GetArcCostForVehicle(previousNode ?? default, node, vehicle);

                this.ActualTotalDistance += transitCost;

                this.RouteDistances[vehicle] = (this.RouteDistances.ElementAt(vehicle) ?? default) + transitCost;
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposing()
        {
            void OnDisposeProblemSolver(TProblemSolver problemSolver)
            {
                if (problemSolver != null)
                {
                    problemSolver.Assign -= this.OnProblemSolverAssign;
                    problemSolver.Dispose();
                }
            }

            OnDisposeProblemSolver(this.ProblemSolver);

            this.Context?.Dispose();

            base.OnDisposing();
        }
    }
}
