using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;
    using DistanceMatrix = Distances.DistanceMatrix;

    /// <summary>
    /// Verifies the initial Vehicle Routing Problem. We verify only the first of the
    /// problems, which does pass last time we checked. But moreover, also aligns with
    /// the <see cref="!:https://developers.google.com/optimization/routing/cvrp">web
    /// site example</see>.
    /// </summary>
    /// <see cref="!:https://developers.google.com/optimization/routing/cvrp"/>
    /// <remarks>Anything further than this, investigating third party distance matrix
    /// API, for instance, is beyond the scope of the unit test verification.</remarks>
    public class ConstraintVehicleRoutingCaseStudyTests : VehicleRoutingCaseStudyTests<ConstraintVehicleRoutingCaseStudyScope>
    {
        public ConstraintVehicleRoutingCaseStudyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Establishes a NodeDemandDimension for use during the routing problem solver.
        /// </summary>
        private class NodeDemandDimension : Dimension
        {
            /// <summary>
            /// Gets the Scope for Private use.
            /// </summary>
            private ConstraintVehicleRoutingCaseStudyScope Scope { get; }

            public NodeDemandDimension(RoutingContext context, ConstraintVehicleRoutingCaseStudyScope scope)
                : base(context, default)
            {
                this.Scope = scope;

                var evaluatorIndex = this.RegisterUnaryTransitEvaluationCallback(this.OnEvaluateTransit);

                this.AddDimension(evaluatorIndex, this.Scope.VehicleCaps.ToArray());
            }

            /// <summary>
            /// Returns the result after Evaluating the
            /// <see cref="ConstraintVehicleRoutingCaseStudyScope.Demands"/> given
            /// <paramref name="node"/>.
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            protected override long OnEvaluateTransit(int node) => this.Scope.Demands.ElementAt(node);
        }

        /// <inheritdoc/>
        protected override ConstraintVehicleRoutingCaseStudyScope OnVerifyInitialScope(ConstraintVehicleRoutingCaseStudyScope scope)
        {
            scope = base.OnVerifyInitialScope(scope);

            scope.ActualTotalDistance.AssertEqual(default);

            scope.Matrix.AssertEqual(scope.Context.NodeCount, x => x.Width);
            scope.Matrix.AssertEqual(scope.Context.NodeCount, x => x.Height);

            scope.Context.AssertEqual(scope.VehicleCount, x => x.VehicleCount);

            return scope;
        }

        /// <summary>
        /// Performs the Background for the CVRP problem.
        /// </summary>
        /// <see cref="!:https://developers.google.com/optimization/routing/cvrp"/>
        [Background]
        public void ConstraintVehicleRoutingBackground()
        {
            $"Add {nameof(NodeDemandDimension)} to this.{nameof(this.Scope)}.{nameof(this.Scope.Context)}".x(
                () => this.Scope.Context.AddDefaultDimension<NodeDemandDimension>(this.Scope)
            );
        }

        /// <summary>
        /// Demonstrates a scenario in which the Solution Agrees with the Optimization Routing
        /// Traveling Salesman Problem illustration.
        /// </summary>
        /// <see cref="!:https://developers.google.com/optimization/routing/cvrp"/>
        /// <see cref="!:https://developers.google.com/optimization/routing/cvrp#entire_program"/>
        [Scenario]
        public void Verify_ProblemSolver_Constraint_Solution()
        {
            $"Solve routing problem given this.{nameof(this.Scope)}.{nameof(this.Scope.Context)}".x(
                () => this.Scope.ProblemSolver.Solve(this.Scope.Context)
            );
        }

        /// <summary>
        /// Verifies that the <paramref name="scope"/> is correct according to what
        /// we expected. We verify given the Optimization Routing TSP Solution.
        /// </summary>
        /// <param name="scope"></param>
        /// <see cref="!:https://developers.google.com/optimization/routing/cvrp#solution"/>
        protected override void OnVerifySolutionScope(ConstraintVehicleRoutingCaseStudyScope scope)
        {
            base.OnVerifySolutionScope(scope);

            scope.SolutionPaths.AssertEqual(scope.VehicleCount, x => x.Count);

            void OnVerifyExpectedPath((int vehicle, int[] expectedPath) tuple) =>
                scope.SolutionPaths[tuple.vehicle].AssertCollectionEqual(tuple.expectedPath);

            scope.ExpectedPaths.AssertEqual(scope.VehicleCount, x => x.Count)
                .Select((x, i) => (i, x)).ToList().ForEach(OnVerifyExpectedPath);
        }
    }
}
