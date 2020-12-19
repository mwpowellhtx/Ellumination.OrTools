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
    public class VehicleRoutingCaseStudyScope : CaseStudyScope<DistanceMatrix
        , RoutingContext, DefaultRoutingAssignmentEventArgs, DefaultRoutingProblemSolver>
    {
        /// <summary>
        /// Gets the VehicleCount, <c>4</c>.
        /// </summary>
        /// <value>4</value>
        internal override int VehicleCount { get; } = 4;

        /// <summary>
        /// Gets the Vehicle Capacity for all vehicles, <c>3000</c>.
        /// </summary>
        /// <value>3000</value>
        internal override long VehicleCap { get; } = 3000;

        // TODO: TBD: we could potentially refactor this sort of initialization to the test fixtures themselves...
        // TODO: TBD: rather than do them here...
        /// <summary>
        /// Gets the ExpectedPaths.
        /// </summary>
        internal override IList<int[]> ExpectedPaths { get; set; } = Range(
            Range(0, 8, 6, 2, 5, 0).ToArray()
            , Range(0, 7, 1, 4, 3, 0).ToArray()
            , Range(0, 9, 10, 16, 14, 0).ToArray()
            , Range(0, 12, 11, 15, 13, 0).ToArray()).ToList();

        private IEnumerable<int?> _expectedRouteDistances;

        /// <summary>
        /// Gets the ExpectedRouteDistances.
        /// </summary>
        internal virtual IEnumerable<int?> ExpectedRouteDistances => this._expectedRouteDistances ?? (
            this._expectedRouteDistances = Range(0, this.VehicleCount).Select(_ => (int?)1552).ToArray()
        );

        // TODO: TBD: what is the proper distance unit?
        /// <summary>
        /// Gets the Vehicle Routing DistanceUnit, <c>&quot;m&quot;</c>
        /// for lack of a better definition.
        /// </summary>
        /// <value>m</value>
        protected override string DistanceUnit { get; } = "m";

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
        protected override DistanceMatrix CreateMatrix() => new DistanceMatrix(this.MatrixValues);

        /// <inheritdoc/>
        protected override RoutingContext CreateContext() => new RoutingContext(this.Matrix.Length, this.VehicleCount, this.Depot);

        /// <inheritdoc/>
        public VehicleRoutingCaseStudyScope(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

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
        /// <see cref="Math.Max(int, int)"/>
        private static int Max(int val1, int val2) => Math.Max(val1, val2);
    }
}
