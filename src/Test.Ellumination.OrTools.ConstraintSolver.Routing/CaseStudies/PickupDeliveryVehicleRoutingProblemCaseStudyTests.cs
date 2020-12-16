using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    // [dotnet::routing] RoutingModel::VehicleIndex type issue / https://github.com/google/or-tools/issues/2282
    // [dotnet::routing] CSharp Context based Wrappers review / https://github.com/google/or-tools/discussions/2249
    // https://developers.google.com/optimization/routing/pickup_delivery
    // https://github.com/mwpowellhtx
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
    public class PickupDeliveryVehicleRoutingCaseStudyTests : OrToolsRoutingCaseStudyTests<
        DistanceMatrix, RoutingContext, DefaultRoutingAssignmentEventArgs
        , DefaultRoutingProblemSolver, PickupDeliveryVehicleRoutingCaseStudyScope>
    {
        public PickupDeliveryVehicleRoutingCaseStudyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <inheritdoc/>
        protected override PickupDeliveryVehicleRoutingCaseStudyScope VerifyScope(PickupDeliveryVehicleRoutingCaseStudyScope scope)
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
        protected override void OnVerifySolution(PickupDeliveryVehicleRoutingCaseStudyScope scope)
        {
            scope.ExpectedPaths.AssertEqual(scope.VehicleCount, x => x.Count);

            //// TODO: TBD: it is less important that the path itself was a match.
            //void OnVerifyExpectedPath((int vehicle, int[] expectedPath) tuple)
            //{
            //    var (vehicle, expectedPath) = tuple;
            //    var actualPath = scope.SolutionPaths[vehicle];
            //    actualPath.AssertCollectionEqual(expectedPath);
            //}
            //scope.ExpectedPaths.Select((x, i) => (i, x)).ToList().ForEach(OnVerifyExpectedPath);

            // It is more important that the pickup/delivery pair is accounted.
            void OnVerifyPickupDelivery((int pickup, int delivery) node)
            {
                bool TryOnVerifyPickupDelivery(int pickup, int delivery, out IEnumerable<int> actualPath)
                {
                    (IList<int> path, int pickupIndex, int deliveryIndex) OnBindSolutionPath(IList<int> path) => (
                        path, pickupIndex: path.IndexOf(pickup), deliveryIndex: path.IndexOf(delivery)
                    );

                    var actualPathMatches = scope.SolutionPaths.Select(OnBindSolutionPath)
                        .Where(_ => _.pickupIndex >= 0 && _.deliveryIndex >= 0)
                        .Where(_ => _.pickupIndex < _.deliveryIndex).ToArray();

                    return (actualPath = actualPathMatches.Select(_ => _.path).SingleOrDefault()) != null;
                }

                TryOnVerifyPickupDelivery(node.pickup, node.delivery, out var _).AssertTrue();
            }

            scope.PickupDeliveries.ToList().ForEach(OnVerifyPickupDelivery);
        }

        private class PrioritizePathDimension : Dimension
        {
            private DistanceMatrix Matrix { get; }

            public PrioritizePathDimension(RoutingContext context, PickupDeliveryVehicleRoutingCaseStudyScope scope)
                : base(context, scope.DimensionCoefficient)
            {
                if (this.Coefficient > 0)
                {
                    this.Matrix = scope.Matrix;

                    var evaluatorIndex = this.RegisterTransitEvaluationCallback(this.OnEvaluateTransit);

                    this.SetArcCostEvaluators(evaluatorIndex);

                    this.AddDimension(evaluatorIndex, scope.VehicleCapacity);

                    this.MutableDimension.SetGlobalSpanCostCoefficient(this.Coefficient);

                    // TODO: TBD: we need to register a callback eval... then the dimension itself...
                    // TODO: TBD: which we also need the distance matrix...
                    this.AddPickupsAndDeliveries(scope.PickupDeliveries);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fromNode"></param>
            /// <param name="toNode"></param>
            /// <returns></returns>
            private int OnEvaluateTransit(int fromNode, int toNode) => this.Matrix[fromNode, toNode] ?? default;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fromIndex"></param>
            /// <param name="toIndex"></param>
            /// <returns></returns>
            private long OnEvaluateTransit(long fromIndex, long toIndex) => this.OnEvaluateTransit(
                this.Context.IndexToNode(fromIndex)
                , this.Context.IndexToNode(toIndex)
            );
        }

        [Background]
        public void PickupDeliveryBackground()
        {
            void OnAddPrioritizePathDimension() => this.Scope.Context.AddDefaultDimension<PrioritizePathDimension>(this.Scope);

            $"Add Default {nameof(PrioritizePathDimension)} dimension".x(OnAddPrioritizePathDimension);
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
