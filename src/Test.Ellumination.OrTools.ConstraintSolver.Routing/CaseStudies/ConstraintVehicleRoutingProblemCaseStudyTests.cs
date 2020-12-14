using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;
    using DistanceMatrix = Distances.DistanceMatrix;
    using static VehicleRoutingCaseStudyScope;

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

        private class ConstraintDimension : Dimension
        {
            private DistanceMatrix Matrix { get; }

            private ConstraintVehicleRoutingCaseStudyScope Scope { get; }

            public ConstraintDimension(RoutingContext context, DistanceMatrix matrix, ConstraintVehicleRoutingCaseStudyScope scope)
                : base(context, default)
            {
                this.Matrix = matrix;
                this.Scope = scope;

                var evaluatorIndex = this.RegisterUnaryTransitEvaluationCallback(this.OnEvaluateTransit);

                this.AddDimension(evaluatorIndex, this.Scope.VehicleCapacities.ToArray());
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            private long OnEvaluateTransit(int node) => this.Scope.Demands.ElementAt(node);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            private long OnEvaluateTransit(long index) => this.OnEvaluateTransit(this.Context.IndexToNode(index));
        }

        /// <inheritdoc/>
        protected override ConstraintVehicleRoutingCaseStudyScope VerifyScope(ConstraintVehicleRoutingCaseStudyScope scope)
        {
            scope = base.VerifyScope(scope);

            scope.TotalDistance.AssertEqual(default);

            scope.Matrix.AssertEqual(scope.Context.NodeCount, x => x.Width);
            scope.Matrix.AssertEqual(scope.Context.NodeCount, x => x.Height);

            scope.Context.AssertEqual(FourVehicles, x => x.VehicleCount);

            scope.SolutionPaths.AssertNotNull().AssertCollectionEmpty();
            scope.RouteDistances.AssertNotNull().AssertCollectionEmpty();

            return scope;
        }

        /// <summary>
        /// Verifies that the <paramref name="scope"/> is correct according to what we expected.
        /// We verify given the Optimization Routing TSP Solution.
        /// </summary>
        /// <param name="scope"></param>
        /// <see cref="!:https://developers.google.com/optimization/routing/cvrp#solution"/>
        protected override void OnVerifySolution(ConstraintVehicleRoutingCaseStudyScope scope)
        {
            scope.SolutionPaths.AssertEqual(FourVehicles, x => x.Count);

            scope.ExpectedPaths.AssertEqual(FourVehicles, x => x.Count);

            void OnVerifyExpectedPath((int[] expectedPath, int vehicle) tuple)
            {
                var (expectedPath, vehicle) = tuple;

                var actualPath = scope.SolutionPaths[vehicle];

                actualPath.AssertCollectionEqual(expectedPath);
            }

            scope.ExpectedPaths.Select((x, i) => (x, i)).ToList().ForEach(OnVerifyExpectedPath);
        }

        // TODO: TBD: assuming that each of the case studies resolve in a sincle "verification" scenario...
        // TODO: TBD: then likely all we need to do is provide that single method...
        /// <summary>
        /// Demonstrates a scenario in which the Solution Agrees with the Optimization Routing
        /// Traveling Salesman Problem illustration.
        /// </summary>
        /// <see cref="!:https://developers.google.com/optimization/routing/cvrp"/>
        [Scenario]
        public void Verify_ProblemSolver_Constraint_Solution()
        {
            // This time this is more Scenario than Background.
            $"Add {nameof(ConstraintDimension)} to this.{nameof(this.Scope)}.{nameof(this.Scope.Context)}".x(
                () => this.Scope.Context.AddDefaultDimension<ConstraintDimension>(this.Scope.Matrix, this.Scope)
            );

            /* See: https://developers.google.com/optimization/routing/cvrp#entire_program, through which
             * we abstract these are the default solver options, i.e. PATH_CHEAPEST_ARC, etc. */

            // TODO: TBD: this is starting to look like really boilerplate stuff as well...
            $"Solve routing problem given this.{nameof(this.Scope)}.{nameof(this.Scope.Context)}".x(
                () => this.Scope.ProblemSolver.Solve(this.Scope.Context)
            );

            void OnVerifySolution() => this.OnVerifySolution(this.Scope);

            $"Verify solution".x(OnVerifySolution);
        }
    }
}
