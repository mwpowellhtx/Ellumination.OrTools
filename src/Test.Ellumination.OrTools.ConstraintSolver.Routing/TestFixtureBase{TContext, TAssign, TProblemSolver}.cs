using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;

    /// <summary>
    /// Test fixture base class.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TAssign"></typeparam>
    /// <typeparam name="TProblemSolver"></typeparam>
    public abstract class TestFixtureBase<TContext, TAssign, TProblemSolver> : TestFixtureBase
        where TContext : Context
        where TAssign : RoutingAssignmentEventArgs<TContext>
        where TProblemSolver : AssignableRoutingProblemSolver<TContext, TAssign>
    {
        /// <summary>
        /// Protected constructor.
        /// </summary>
        /// <param name="outputHelper"></param>
        protected TestFixtureBase(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Callback occurs on <see cref="Context"/> having been Created.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual TContext OnContextCreated(TContext context)
        {
            context.AssertNotNull();
            return context;
        }

        /// <summary>
        /// Returns a Created <typeparamref name="TContext"/>.
        /// </summary>
        /// <returns></returns>
        protected abstract TContext CreateContext();

        private TContext _context;

        /// <summary>
        /// Gets the Context.
        /// </summary>
        protected virtual TContext Context => this._context ?? (
            this._context = this.OnContextCreated(this.CreateContext())
        );

        /// <summary>
        /// Callback occurs On <see cref="ProblemSolver"/> having been Created.
        /// </summary>
        /// <param name="problemSolver"></param>
        /// <returns></returns>
        protected virtual TProblemSolver OnProblemSolverCreated(TProblemSolver problemSolver)
        {
            problemSolver.AssertNotNull();

            problemSolver.Assigning += this.OnAssigning;
            problemSolver.Assign += this.OnAssignOrAssigned;
            problemSolver.Assigned += this.OnAssignOrAssigned;

            return problemSolver;
        }

        /// <summary>
        /// Returns a Created <typeparamref name="TProblemSolver"/>.
        /// </summary>
        /// <returns></returns>
        protected abstract TProblemSolver CreateProblemSolver();

        private TProblemSolver _problemSolver;

        /// <summary>
        /// Gets the ProblemSolver.
        /// </summary>
        protected virtual TProblemSolver ProblemSolver => this._problemSolver ?? (
            this._problemSolver = this.OnProblemSolverCreated(this.CreateProblemSolver())
        );

        /// <summary>
        /// Gets the Solution for Private usage.
        /// </summary>
        private ICollection<(int vehicle, int node)> PrivateSolution { get; } = new List<(int vehicle, int node)>();

        /// <summary>
        /// Gets the Solution for use throughout the scenarios.
        /// </summary>
        /// <remarks>It might actually be better for subscribers to host domain facing
        /// collections such as these in their routing model problem solver derived
        /// implementations for convenience, especially adapting to domain model.</remarks>
        protected virtual IEnumerable<(int vehicle, int node)> Solution => this.PrivateSolution.AssertNotNull();

        /// <summary>
        /// Event handler callback On
        /// <see cref="IAssignableRoutingProblemSolver{TContext, TAssign}.Assigning"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAssigning(object sender, TAssign e)
        {
            // TODO: TBD: just making sure we are connect the dots properly...
            // TODO: TBD: these are obviously going to fail...
            this.PrivateSolution.Add((e.VehicleIndex, e.NodeIndex));
        }

        /// <summary>
        /// Event handler callback On
        /// <see cref="IAssignableRoutingProblemSolver{TContext, TAssign}.Assign"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAssignOrAssigned(object sender, TAssign e)
        {
            void VerifyLastSolution((int vehicle, int node) pair)
            {
                pair.vehicle.AssertEqual(e.VehicleIndex);
                pair.node.AssertEqual(e.NodeIndex);
            }

            VerifyLastSolution(this.PrivateSolution.AssertTrue(x => x.Count > 0).Last());
        }

        /// <summary>
        /// Override in order to Verify the <paramref name="context"/>.
        /// <see cref="Context.Depot"/> and <see cref="Context.DepotCoordinates"/>
        /// may or may not be set given usage under test.
        /// </summary>
        /// <param name="context"></param>
        protected virtual TContext VerifyContext(TContext context)
        {
            context.AssertNotNull();

            context.VehicleCount.AssertTrue(x => x > 0);

            context.NodeCount.AssertTrue(x => x > 0);

            context.InternalDimensions.AssertNotNull().AssertCollectionEmpty();

            void OnVerifyContextEndpoints(int expectedVehicleCount, params (int start, int end)[] eps)
            {
                eps.AssertEqual(expectedVehicleCount, x => x.Length);

                IEnumerable<int> OnDecoupleEndpoint((int start, int end) ep) => ep.DecoupleEndpoint();

                bool OnEndpointCoordValid(int actualCoord) => actualCoord >= 0 && actualCoord < context.VehicleCount;

                bool AreEndpointCoordsValid(IEnumerable<int> coords) => coords.All(OnEndpointCoordValid);

                eps.SelectMany(OnDecoupleEndpoint).AssertTrue(AreEndpointCoordsValid);
            }

            OnVerifyContextEndpoints(context.VehicleCount, context.Endpoints.AssertNotNull().ToArray());

            context.Manager.AssertNotNull();

            context.Model.AssertNotNull();

            return context;
        }

        [Background]
        public void BaseBackground()
        {
            "Initialize the base class".x(() => true.AssertTrue());

            $"this.{nameof(Context)} is initialized".x(() => this.Context.AssertNotNull());

            $"this.{nameof(ProblemSolver)} is initialized".x(() => this.ProblemSolver.AssertNotNull());
        }

        /// <summary>
        /// Override in order to Verify the <paramref name="solution"/>.
        /// </summary>
        /// <param name="solution"></param>
        protected virtual void OnVerifySolution(params (int vehicle, int node)[] solution)
        {
        }

        [Scenario]
        public void ProblemSolver_Does_Solve()
        {
            void OnSolveProblem() => this.ProblemSolver.Solve(this.Context);

            "Solve the problem".x(OnSolveProblem);

            $"Verify this.{nameof(Solution)}".x(() => this.OnVerifySolution(this.Solution.ToArray()));
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                void OnProblemSolverDispose(TProblemSolver problemSolver)
                {
                    if (problemSolver != null)
                    {
                        problemSolver.Assigning -= this.OnAssigning;
                        problemSolver.Assign -= this.OnAssignOrAssigned;
                        problemSolver.Assigned -= this.OnAssignOrAssigned;
                    }

                    problemSolver?.Dispose();
                }

                OnProblemSolverDispose(this.ProblemSolver);

                this.Context?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
