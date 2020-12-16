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
    /// the <see cref="!:https://developers.google.com/optimization/routing/vrp">web
    /// site example</see>.
    /// </summary>
    /// <see cref="!:https://developers.google.com/optimization/routing/vrp"/>
    /// <remarks>Anything further than this, investigating third party distance matrix
    /// API, for instance, is beyond the scope of the unit test verification.</remarks>
    public class VehicleRoutingCaseStudyTests : VehicleRoutingCaseStudyTests<VehicleRoutingCaseStudyScope>
    {
        public VehicleRoutingCaseStudyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        private class ArcCostDimension : Dimension
        {
            private DistanceMatrix Matrix { get; }

            public ArcCostDimension(RoutingContext context, DistanceMatrix matrix)
                : base(context, default)
            {
                this.Matrix = matrix;

                var evaluatorIndex = this.RegisterTransitEvaluationCallback(this.OnEvaluateTransit);

                this.SetArcCostEvaluators(evaluatorIndex);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fromNode"></param>
            /// <param name="toNode"></param>
            /// <returns></returns>
            private long OnEvaluateTransit(int fromNode, int toNode) => this.Matrix[fromNode, toNode] ?? default;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fromIndex"></param>
            /// <param name="toIndex"></param>
            /// <returns></returns>
            private long OnEvaluateTransit(long fromIndex, long toIndex) => this.OnEvaluateTransit(
                this.Context.IndexToNode(fromIndex), this.Context.IndexToNode(toIndex)
            );
        }

        private class DistanceDimension : Dimension
        {
            /// <summary>
            /// Gets the DefaultCoefficient, defaults to <c>100</c>.
            /// </summary>
            private static int DefaultCoefficient { get; } = 100;

            private DistanceMatrix Matrix { get; }

            public DistanceDimension(RoutingContext context, DistanceMatrix matrix)
                : base(context, DefaultCoefficient)
            {
                this.Matrix = matrix;

                if (this.Coefficient > 0)
                {
                    var evaluatorIndex = this.RegisterTransitEvaluationCallback(this.OnEvaluateTransit);

                    this.AddDimension(evaluatorIndex, 3000);

                    var this_MutableDim = this.MutableDimension;

                    this_MutableDim.SetGlobalSpanCostCoefficient(this.Coefficient);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fromNode"></param>
            /// <param name="toNode"></param>
            /// <returns></returns>
            private long OnEvaluateTransit(int fromNode, int toNode) => this.Matrix[fromNode, toNode] ?? default;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fromIndex"></param>
            /// <param name="toIndex"></param>
            /// <returns></returns>
            private long OnEvaluateTransit(long fromIndex, long toIndex) => this.OnEvaluateTransit(
                this.Context.IndexToNode(fromIndex), this.Context.IndexToNode(toIndex)
            );
        }

        /// <inheritdoc/>
        protected override VehicleRoutingCaseStudyScope VerifyScope(VehicleRoutingCaseStudyScope scope)
        {
            scope = base.VerifyScope(scope);

            scope.TotalDistance.AssertEqual(default);

            scope.Matrix.AssertEqual(scope.Context.NodeCount, x => x.Width);
            scope.Matrix.AssertEqual(scope.Context.NodeCount, x => x.Height);

            scope.Context.AssertEqual(scope.VehicleCount, x => x.VehicleCount);

            return scope;
        }

        /// <summary>
        /// Verifies that the <paramref name="scope"/> is correct according to what we expected.
        /// We verify given the Optimization Routing TSP Solution.
        /// </summary>
        /// <param name="scope"></param>
        /// <see cref="!:https://developers.google.com/optimization/routing/vrp#solution"/>
        protected override void OnVerifySolution(VehicleRoutingCaseStudyScope scope)
        {
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
        /// <see cref="!:https://developers.google.com/optimization/routing/vrp"/>
        [Scenario]
        public void Verify_ProblemSolver_Solution()
        {
            /* See: https://developers.google.com/optimization/routing/vrp#entire_program1, through which
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
