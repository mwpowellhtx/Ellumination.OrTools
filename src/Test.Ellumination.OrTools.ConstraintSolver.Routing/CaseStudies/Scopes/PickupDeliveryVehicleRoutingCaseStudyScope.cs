using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using DistanceMatrix = Distances.DistanceMatrix;
    using static TestFixtureBase;

    /// <inheritdoc/>
    public class PickupDeliveryVehicleRoutingCaseStudyScope : VehicleRoutingCaseStudyScope
    {
        /// <summary>
        /// Gets the DimensionCoefficient, <c>100</c>.
        /// </summary>
        internal int DimensionCoefficient { get; } = 100;

        /// <summary>
        /// Gets the VehicleCapacity, <c>3000</c>.
        /// </summary>
        internal long VehicleCapacity { get; } = 3000;

        /// <summary>
        /// Gets the Nodes From which Pickups must be made, paired with the Nodes To which
        /// Deliveries must be made. The pairings are an absolute requirement in the model,
        /// and that the Deliveries must occur after their respective Pickups, conversely
        /// that Pickups must occur sooner than their respective Deliveries.
        /// </summary>
        internal virtual IEnumerable<(int pickup, int delivery)> PickupDeliveries { get; } = Range(
            (1, 6), (2, 10), (4, 3), (5, 9), (7, 8), (15, 11), (13, 12), (16, 14)
        ).ToArray();

        private ICollection<int[]> _expectedPaths;

        /// <inheritdoc/>
        internal override ICollection<int[]> ExpectedPaths => this._expectedPaths ?? (this._expectedPaths
            = Range(
                Range(0, 13, 15, 11, 12, 0).ToArray()
                , Range(0, 5, 2, 10, 16, 14, 9, 0).ToArray()
                , Range(0, 4, 3, 0).ToArray()
                , Range(0, 7, 1, 6, 8, 0).ToArray()).ToList()).AssertEqual(this.VehicleCount, x => x.Count);

        /// <inheritdoc/>
        internal override IEnumerable<int?> ExpectedRouteDistances { get; } = Range<int?>(1780, 1780, 2032, 1712);

        /// <summary>
        /// Constructs a Case Study Scope instance.
        /// </summary>
        /// <param name="outputHelper"></param>
        public PickupDeliveryVehicleRoutingCaseStudyScope(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        // TODO: TBD: what is the proper distance unit?
        /// <summary>
        /// Gets the Vehicle Routing DistanceUnit, &quot;m&quot; for lack of a better definition.
        /// </summary>
        protected override string DistanceUnit { get; } = "m";

        private DistanceMatrix _matrix;

        private RoutingContext _context;

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
        internal override DistanceMatrix Matrix => this._matrix ?? (this._matrix
            = new DistanceMatrix(this.MatrixValues)
        );

        /// <inheritdoc/>
        internal override RoutingContext Context => this._context ?? (this._context
            = new RoutingContext(this.Matrix.Length, this.VehicleCount, Depot)
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

        //// TODO: TBD: probably unnecessary from a scope perspective...
        //protected override void OnProblemSolverAssign(object sender, DefaultRoutingAssignmentEventArgs e)
        //{
        //    base.OnProblemSolverAssign(sender, e);
        //}

        /// <summary>
        /// Verifies the Solution vis a vis the TSP solution.
        /// </summary>
        /// <see cref="!:https://developers.google.com/optimization/routing/vrp#solution"/>
        internal override void VerifySolution()
        {
            //base.VerifySolution();

            this.AssertEqual(7304, x => x.TotalDistance);

            this.RouteDistances.AssertCollectionEqual(this.ExpectedRouteDistances);

            this.SolutionPaths.AssertEqual(this.VehicleCount, x => x.Count);

            //// TODO: TBD: I think that we verification may be duplicate between scope and the actual tests...
            //void OnVerifyVehicle(int vehicle)
            //{
            //    // Pretty much verbatim, https://developers.google.com/optimization/routing/vrp#solution.
            //    var actualPath = this.SolutionPaths[vehicle].AssertNotNull();
            //    var expectedPath = this.ExpectedPaths.ElementAt(vehicle);
            //    actualPath.AssertCollectionEqual(expectedPath);
            //}
            //Range(0, this.VehicleCount).ToList().ForEach(OnVerifyVehicle);

            /* See: https://developers.google.com/optimization/routing/vrp#printer, which
             * we should have the solution in hand approaching test disposal. */

            void OnReportPickupDelivery((int index, int pickup, int delivery) nodeIndex)
            {
                const char comma = ',';

                var (i, pickup, delivery) = nodeIndex;

                var rendered = RenderTupleAssociates(
                    (nameof(i), Render(i))
                    , (nameof(pickup), Render(pickup))
                    , (nameof(delivery), Render(delivery))
                );

                this.OutputHelper.WriteLine($"  {(i == 0 ? string.Empty : $"{comma} ")}{rendered}");
            }

            void OnReportPickupsDeliveries(params (int pickup, int delivery)[] nodes)
            {
                const string squareBrackets = "[]";
                const string curlyBraces = "{}";
                this.OutputHelper.WriteLine($"{nameof(this.PickupDeliveries)}{squareBrackets} = {curlyBraces[0]}");
                nodes.Select((_, i) => (index: i, _.pickup, _.delivery)).ToList().ForEach(OnReportPickupDelivery);
                this.OutputHelper.WriteLine($"{curlyBraces[1]}");
            }

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

            OnReportPickupsDeliveries(this.PickupDeliveries.ToArray());

            Range(0, this.VehicleCount)
                .Select(_ => (_, this.SolutionPaths[_].AssertIsAssignableFrom<IEnumerable<int>>()))
                    .ToList().ForEach(OnReportEachVehiclePath);

            OnReportTotalDistance(this.TotalDistance);
        }
    }
}
