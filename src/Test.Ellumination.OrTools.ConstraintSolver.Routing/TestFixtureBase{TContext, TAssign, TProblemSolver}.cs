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
        where TContext : RoutingContext
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
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <param name="assignments"></param>
        protected virtual void VerifyAssignment((int vehicle, int node, int? previousNode) current
            , IEnumerable<(int vehicle, int node, int? previousNode)> assignments)
        {
            // TODO: TBD: verify other bits?
            assignments.Contains(current).AssertTrue();
        }

        /// <summary>
        /// <typeparamref name="TProblemSolver"/> <see cref="TAssign"/> event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnProblemSolverBeforeAssignment(object sender, TAssign e)
        {
            this.PrivateAllAssignments.Clear();
        }

        /// <summary>
        /// <typeparamref name="TProblemSolver"/> <see cref="TAssign"/> event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnProblemSolverAfterAssignment(object sender, TAssign e)
        {
            // TODO: TBD: this is it...
            e.Assignments.AssertNotNull().ToList().ForEach(_ => this.VerifyAssignment(_, this.AllAssignments));
        }

        /// <summary>
        /// <typeparamref name="TProblemSolver"/> <see cref="TAssign"/> event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnProblemSolverBeforeAssignmentVehicle(object sender, TAssign e)
        {
            this.PrivateVehicleAssignments.Clear();
        }

        /// <summary>
        /// <typeparamref name="TProblemSolver"/> <see cref="TAssign"/> event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnProblemSolverAfterAssignmentVehicle(object sender, TAssign e)
        {
            // TODO: TBD: ditto, more to verify here?
            e.Assignments.AssertNotNull().ToList().ForEach(_ => this.VerifyAssignment(_, this.VehicleAssignments));
        }

        /// <summary>
        /// <typeparamref name="TProblemSolver"/> <see cref="TAssign"/> event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnProblemSolverForEachAssignmentNode(object sender, TAssign e)
        {
            var e_Assignments = e.Assignments.AssertNotNull().ToArray();

            this.PrivateForEachAssignments.Clear();
            this.PrivateForEachAssignments.AddRange(e_Assignments);

            this.PrivateVehicleAssignments.AddRange(e_Assignments);
            this.PrivateAllAssignments.AddRange(e_Assignments);
        }

        /// <summary>
        /// Callback occurs On <see cref="ProblemSolver"/> having been Created.
        /// </summary>
        /// <param name="problemSolver"></param>
        /// <returns></returns>
        protected virtual TProblemSolver OnProblemSolverCreated(TProblemSolver problemSolver)
        {
            problemSolver.AssertNotNull();

            problemSolver.BeforeAssignment += this.OnProblemSolverBeforeAssignment;
            problemSolver.AfterAssignment += this.OnProblemSolverAfterAssignment;

            problemSolver.BeforeAssignmentVehicle += this.OnProblemSolverBeforeAssignmentVehicle;
            problemSolver.AfterAssignmentVehicle += this.OnProblemSolverAfterAssignmentVehicle;

            problemSolver.ForEachAssignmentNode += this.OnProblemSolverForEachAssignmentNode;

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
        /// Gets the Assignments for Private use.
        /// </summary>
        /// <see cref="AllAssignments"/>
        private List<(int vehicle, int node, int? previousNode)> PrivateAllAssignments { get; } = Range<(int, int, int?)>().ToList();

        /// <summary>
        /// Gets the Assignments for Private use.
        /// </summary>
        /// <see cref="VehicleAssignments"/>
        private List<(int vehicle, int node, int? previousNode)> PrivateVehicleAssignments { get; } = Range<(int, int, int?)>().ToList();

        /// <summary>
        /// Gets the Assignments for Private use.
        /// </summary>
        /// <see cref="ForEachAssignments"/>
        private List<(int vehicle, int node, int? previousNode)> PrivateForEachAssignments { get; } = Range<(int, int, int?)>().ToList();

        /// <summary>
        /// Gets the Assignments for Protected use.
        /// </summary>
        /// <see cref="PrivateAllAssignments"/>
        protected virtual IEnumerable<(int vehicle, int node, int? previousNode)> AllAssignments => this.PrivateAllAssignments;

        /// <summary>
        /// Gets the Assignments for Protected use.
        /// </summary>
        /// <see cref="PrivateVehicleAssignments"/>
        protected virtual IEnumerable<(int vehicle, int node, int? previousNode)> VehicleAssignments => this.PrivateVehicleAssignments;

        /// <summary>
        /// Gets the Assignments for Protected use.
        /// </summary>
        /// <see cref="PrivateForEachAssignments"/>
        protected virtual IEnumerable<(int vehicle, int node, int? previousNode)> ForEachAssignments => this.PrivateForEachAssignments;

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
        /// Override in order to Verify the <paramref name="assignments"/>.
        /// </summary>
        /// <param name="assignments"></param>
        protected virtual void OnVerifyAssignments(params (int vehicle, int node, int? previousNode)[] assignments)
        {
        }

        [Scenario]
        public void ProblemSolver_Does_Solve()
        {
            void OnSolveProblem() => this.ProblemSolver.Solve(this.Context);

            "Solve the problem".x(OnSolveProblem);

            $"Verify this.{nameof(AllAssignments)}".x(() => this.OnVerifyAssignments(this.AllAssignments.ToArray()));
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
                        problemSolver.BeforeAssignment -= this.OnProblemSolverBeforeAssignment;
                        problemSolver.AfterAssignment -= this.OnProblemSolverAfterAssignment;

                        problemSolver.BeforeAssignmentVehicle -= this.OnProblemSolverBeforeAssignmentVehicle;
                        problemSolver.AfterAssignmentVehicle -= this.OnProblemSolverAfterAssignmentVehicle;

                        problemSolver.ForEachAssignmentNode -= this.OnProblemSolverForEachAssignmentNode;
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
