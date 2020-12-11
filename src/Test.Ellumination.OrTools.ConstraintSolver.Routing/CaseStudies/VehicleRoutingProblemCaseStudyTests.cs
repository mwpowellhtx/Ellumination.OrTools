using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;
    using DistanceMatrix = Distances.DistanceMatrix;
    using static VehicleRoutingCaseStudyTests.VehicleRoutingCaseStudyScope;

    /// <summary>
    /// Verifies the initial Vehicle Routing Problem. We verify only the first of the
    /// problems, which does pass last time we checked. But moreover, also aligns with
    /// the <see cref="!:https://developers.google.com/optimization/routing/vrp">web
    /// site example</see>.
    /// </summary>
    /// <see cref="!:https://developers.google.com/optimization/routing/vrp"/>
    /// <remarks>Anything further than this, investigating third party distance matrix
    /// API, for instance, is beyond the scope of the unit test verification.</remarks>
    public class VehicleRoutingCaseStudyTests : OrToolsRoutingCaseStudyTests<DistanceMatrix
        , RoutingContext, VehicleRoutingCaseStudyTests.VehicleRoutingCaseStudyScope>
    {
        public VehicleRoutingCaseStudyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <inheritdoc/>
        public class VehicleRoutingCaseStudyScope : CaseStudyScope
        {
            /// <summary>
            /// Gets the ExpectedPaths.
            /// </summary>
            internal static ICollection<int[]> ExpectedPaths = Range(
                Range(0, 8, 6, 2, 5, 0).ToArray()
                , Range(0, 7, 1, 4, 3, 0).ToArray()
                , Range(0, 9, 10, 16, 14, 0).ToArray()
                , Range(0, 12, 11, 15, 13, 0).ToArray()).ToList();

            /// <summary>
            /// Gets the ExpectedRouteDistances.
            /// </summary>
            internal static IEnumerable<int> ExpectedRouteDistances => Enumerable.Range(0, FourVehicles).Select(_ => 1552).ToArray();

            /// <summary>
            /// Constructs a Case Study Scope instance.
            /// </summary>
            /// <param name="outputHelper"></param>
            public VehicleRoutingCaseStudyScope(ITestOutputHelper outputHelper)
                : base(outputHelper)
            {
            }

            // TODO: TBD: what is the proper distance unit?
            /// <summary>
            /// Gets the Vehicle Routing DistanceUnit, &quot;units&quot;
            /// for lack of a better definition.
            /// </summary>
            protected override string DistanceUnit { get; } = "units";

            private DistanceMatrix _matrix;

            private RoutingContext _context;

            /// <summary>
            /// Vehicle routing, we illustrate <c>4</c> Vehicles.
            /// </summary>
            internal const int FourVehicles = 4;

            /// <summary>
            /// Zed, <c>0</c>, is the home Depot.
            /// </summary>
            private const int Depot = 0;

            /// <inheritdoc/>
            protected override int?[,] MatrixValues { get; } = {
                { 0, 548, 776, 696, 582, 274, 502, 194, 308, 194, 536, 502, 388, 354, 468, 776, 662 }
                , { 548, 0, 684, 308, 194, 502, 730, 354, 696, 742, 1084, 594, 480, 674, 1016, 868, 1210 }
                , { 776, 684, 0, 992, 878, 502, 274, 810, 468, 742, 400, 1278, 1164, 1130, 788, 1552, 754 }
                , { 696, 308, 992, 0, 114, 650, 878, 502, 844, 890, 1232, 514, 628, 822, 1164, 560, 1358 }
                , { 582, 194, 878, 114, 0, 536, 764, 388, 730, 776, 1118, 400, 514, 708, 1050, 674, 1244 }
                , { 274, 502, 502, 650, 536, 0, 228, 308, 194, 240, 582, 776, 662, 628, 514, 1050, 708 }
                , { 502, 730, 274, 878, 764, 228, 0, 536, 194, 468, 354, 1004, 890, 856, 514, 1278, 480 }
                , { 194, 354, 810, 502, 388, 308, 536, 0, 342, 388, 730, 468, 354, 320, 662, 742, 856 }
                , { 308, 696, 468, 844, 730, 194, 194, 342, 0, 274, 388, 810, 696, 662, 320, 1084, 514 }
                , { 194, 742, 742, 890, 776, 240, 468, 388, 274, 0, 342, 536, 422, 388, 274, 810, 468 }
                , { 536, 1084, 400, 1232, 1118, 582, 354, 730, 388, 342, 0, 878, 764, 730, 388, 1152, 354 }
                , { 502, 594, 1278, 514, 400, 776, 1004, 468, 810, 536, 878, 0, 114, 308, 650, 274, 844 }
                , { 388, 480, 1164, 628, 514, 662, 890, 354, 696, 422, 764, 114, 0, 194, 536, 388, 730 }
                , { 354, 674, 1130, 822, 708, 628, 856, 320, 662, 388, 730, 308, 194, 0, 342, 422, 536 }
                , { 468, 1016, 788, 1164, 1050, 514, 514, 662, 320, 274, 388, 650, 536, 342, 0, 764, 194 }
                , { 776, 868, 1552, 560, 674, 1050, 1278, 742, 1084, 810, 1152, 274, 388, 422, 764, 0, 798 }
                , { 662, 1210, 754, 1358, 1244, 708, 480, 856, 514, 468, 354, 844, 730, 536, 194, 798, 0 }
            };

#pragma warning disable IDE0051 // Private member {member} is unused
            /// <summary>
            /// Gets the Coordinates.
            /// </summary>
            /// <see cref="Depot"/>
            /// <remarks>Unused for purposes of this example, but it is mentioned on the web
            /// site, so we include it here, with possibly future use as the caveat.</remarks>
            private IEnumerable<(int x, int y)> Coordinates { get; } = Range<(int x, int y)>(
                (456, 320)
                , (228, 0)
                , (912, 0)
                , (0, 80)
                , (114, 80)
                , (570, 160)
                , (798, 160)
                , (342, 240)
                , (684, 240)
                , (570, 400)
                , (912, 400)
                , (114, 480)
                , (228, 480)
                , (342, 560)
                , (684, 560)
                , (0, 640)
                , (798, 640)
            );

            /// <inheritdoc/>
            internal override DistanceMatrix Matrix => this._matrix ?? (this._matrix = new DistanceMatrix(this.MatrixValues));

            /// <inheritdoc/>
            internal override RoutingContext Context => this._context ?? (
                this._context = new RoutingContext(this.Matrix.Length, FourVehicles, Depot)
            );

            /// <summary>
            /// Returns the Maximum of the <paramref name="val1"/> and <paramref name="val2"/>
            /// values.
            /// </summary>
            /// <param name="val1"></param>
            /// <param name="val2"></param>
            /// <returns></returns>
            /// <remarks>Note that we included this here because a <see cref="Math.Max(int, int)"/>
            /// reference was included in the web site example, but we do not find any evidence
            /// of this in the subsequent report.</remarks>
            private static int Max(int val1, int val2) => Math.Max(val1, val2);

            protected override void OnProblemSolverAssign(object sender, DefaultRoutingAssignmentEventArgs e)
            {
                var (vehicle, node, previousNode) = (e.AssertNotNull().VehicleIndex, e.NodeIndex, e.PreviousNodeIndex);

                // Vehicle should always be this.
                vehicle.AssertTrue(x => x >= 0 && x < FourVehicles);

                this.SolutionPaths[vehicle] = this.SolutionPaths.TryGetValue(vehicle, out var path)
                    ? path
                    : new List<int>();

                this.SolutionPaths[vehicle].AssertNotNull().Add(node);

                if (previousNode.HasValue)
                {
                    var this_Context_ArcCost_node = (int)this.Context.GetArcCostForVehicle(previousNode.Value, node, vehicle);

                    this.TotalDistance += this_Context_ArcCost_node;

                    // We will keep these two lines separate, which makes for easier debugging when necessary.
                    var tried = this.RouteDistances.TryGetValue(vehicle, out var this_RouteDistances_value);

                    this.RouteDistances[vehicle] = (tried ? this_RouteDistances_value : default(int)) + this_Context_ArcCost_node;
                }

                base.OnProblemSolverAssign(sender, e);
            }

            /// <summary>
            /// Verifies the Solution vis a vis the TSP solution.
            /// </summary>
            /// <see cref="!:https://developers.google.com/optimization/routing/vrp#solution"/>
            internal override void VerifySolution()
            {
                base.VerifySolution();

                this.AssertEqual(6208, x => x.TotalDistance);

                this.SolutionPaths.AssertEqual(FourVehicles, x => x.Count);

                void OnVerifyVehicle(int vehicle)
                {
                    if (this.SolutionPaths.TryGetValue(vehicle, out var actualPath).AssertTrue())
                    {
                        // Pretty much verbatim, https://developers.google.com/optimization/routing/vrp#solution.

                        var expectedPath = ExpectedPaths.ElementAt(vehicle);

                        actualPath.AssertCollectionEqual(expectedPath);
                    }
                }

                Enumerable.Range(0, FourVehicles).ToList().ForEach(OnVerifyVehicle);

                this.RouteDistances.Values.Select(x => (int)x.AssertNotNull()).AssertCollectionEqual(ExpectedRouteDistances);

                /* See: https://developers.google.com/optimization/routing/vrp#printer, which
                 * we should have the solution in hand approaching test disposal. */

                void OnReportTotalDistance(int totalDistance) =>
                    this.OutputHelper.WriteLine($"Total distance of all routes: {totalDistance} {this.DistanceUnit}");

                void OnReportEachVehiclePath((int vehicle, IEnumerable<int> path) item)
                {
                    this.OutputHelper.WriteLine(
                        $"Route for vehicle {item.vehicle}: {string.Join(" -> ", item.path)}"
                    );

                    this.OutputHelper.WriteLine(
                        $"Distance of route: {this.RouteDistances[item.vehicle] ?? default}"
                    );

                    this.OutputHelper.WriteLine(string.Empty);
                }

                this.SolutionPaths.Keys.OrderBy(vehicle => vehicle)
                    .Select(vehicle => (vehicle, (IEnumerable<int>)this.SolutionPaths[vehicle]))
                        .ToList().ForEach(OnReportEachVehiclePath);

                OnReportTotalDistance(this.TotalDistance);
            }
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
        /// <see cref="!:https://developers.google.com/optimization/routing/vrp#solution"/>
        protected override void OnVerifySolution(VehicleRoutingCaseStudyScope scope)
        {
            scope.SolutionPaths.AssertEqual(FourVehicles, x => x.Count);

            ExpectedPaths.AssertEqual(FourVehicles, x => x.Count);

            void OnVerifyExpectedPath((int[] expectedPath, int vehicle) tuple)
            {
                var (expectedPath, vehicle) = tuple;

                var actualPath = scope.SolutionPaths[vehicle];

                actualPath.AssertCollectionEqual(expectedPath);
            }

            ExpectedPaths.Select((x, i) => (x, i)).ToList().ForEach(OnVerifyExpectedPath);
        }

        /// <summary>
        /// Arranges for the Vehicle Routing Case Study scenarios to be performed.
        /// </summary>
        [Background]
        public void VehicleRoutingBackground()
        {
            $"Add {nameof(ArcCostDimension)} to this.{nameof(this.Scope)}.{nameof(this.Scope.Context)}".x(
                () => this.Scope.Context.AddDefaultDimension<ArcCostDimension>(this.Scope.Matrix)
            );

            $"Add {nameof(DistanceDimension)} to this.{nameof(this.Scope)}.{nameof(this.Scope.Context)}".x(
                () => this.Scope.Context.AddDefaultDimension<DistanceDimension>(this.Scope.Matrix)
            );
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
