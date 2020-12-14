using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using DistanceMatrix = Distances.DistanceMatrix;
    using static TestFixtureBase;

    /// <inheritdoc/>
    public class ConstraintVehicleRoutingCaseStudyScope : VehicleRoutingCaseStudyScope
    {
        /// <summary>
        /// Gets the Demands informing the Case Study tests.
        /// </summary>
        public IEnumerable<long> Demands { get; } = Range<long>(
            0, 1, 1, 2, 4, 2, 4, 8, 8, 1, 2, 1, 2, 4, 4, 8, 8).ToArray();

        /// <summary>
        /// Gets the VehicleCapacities informing the Case Study tests.
        /// </summary>
        internal virtual IEnumerable<long> VehicleCapacities { get; }
            = Enumerable.Range(0, FourVehicles).Select(_ => 15L).ToArray();


        /// <inheritdoc/>
        internal override ICollection<int[]> ExpectedPaths { get; } = Range(
            Range(0, 1, 4, 3, 15, 0).ToArray()
            , Range(0, 14, 16, 10, 2, 0).ToArray()
            , Range(0, 7, 13, 12, 11, 0).ToArray()
            , Range(0, 9, 8, 6, 5, 0).ToArray()).ToList();

        /// <inheritdoc/>
        internal override IEnumerable<int> ExpectedRouteDistances { get; } = Range(2192, 2192, 1324, 1164);

        /// <summary>
        /// Constructs a Case Study Scope instance.
        /// </summary>
        /// <param name="outputHelper"></param>
        public ConstraintVehicleRoutingCaseStudyScope(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        // TODO: TBD: what is the proper distance unit?
        /// <summary>
        /// Gets the Vehicle Routing DistanceUnit, &quot;units&quot;
        /// for lack of a better definition.
        /// </summary>
        protected override string DistanceUnit { get; } = "m";

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

            //base.OnProblemSolverAssign(sender, e);
        }

        /// <summary>
        /// Verifies the Solution vis a vis the TSP solution.
        /// </summary>
        /// <see cref="!:https://developers.google.com/optimization/routing/cvrp#solution"/>
        internal override void VerifySolution()
        {
            //base.VerifySolution();

            this.AssertEqual(6872, x => x.TotalDistance);

            this.SolutionPaths.AssertEqual(FourVehicles, x => x.Count);

            void OnVerifyVehicle(int vehicle)
            {
                if (this.SolutionPaths.TryGetValue(vehicle, out var actualPath).AssertTrue())
                {
                    // Pretty much verbatim, https://developers.google.com/optimization/routing/cvrp#solution.

                    var expectedPath = ExpectedPaths.ElementAt(vehicle);

                    actualPath.AssertCollectionEqual(expectedPath);
                }
            }

            Enumerable.Range(0, FourVehicles).ToList().ForEach(OnVerifyVehicle);

            this.RouteDistances.Values.Select(x => (int)x.AssertNotNull()).AssertCollectionEqual(ExpectedRouteDistances);

            /* See: https://developers.google.com/optimization/routing/cvrp#printer, which
             * we should have the solution in hand approaching test disposal. */

            void OnReportTotalDistance(int totalDistance) =>
                this.OutputHelper.WriteLine($"Total distance of all routes: {totalDistance} {this.DistanceUnit}");

            void OnReportEachVehiclePath((int vehicle, IEnumerable<int> path) item)
            {
                this.OutputHelper.WriteLine(
                    $"Route for vehicle {item.vehicle}: {string.Join(" -> ", item.path)}"
                );

                this.OutputHelper.WriteLine(
                    $"Distance of route: {this.RouteDistances[item.vehicle] ?? default} {this.DistanceUnit}"
                );

                this.OutputHelper.WriteLine(string.Empty);
            }

            this.SolutionPaths.Keys.OrderBy(vehicle => vehicle)
                .Select(vehicle => (vehicle, (IEnumerable<int>)this.SolutionPaths[vehicle]))
                    .ToList().ForEach(OnReportEachVehiclePath);

            OnReportTotalDistance(this.TotalDistance);
        }
    }
}
