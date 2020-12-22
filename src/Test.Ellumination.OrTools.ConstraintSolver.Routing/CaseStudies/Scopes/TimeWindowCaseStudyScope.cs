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
    public class TimeWindowCaseStudyScope : CaseStudyScope<DistanceMatrix
        , RoutingContext, DefaultRoutingAssignmentEventArgs, DefaultRoutingProblemSolver>
    {
        /// <summary>
        /// Gets the VehicleCount, <c>4</c>.
        /// </summary>
        /// 
        /// <value>4</value>
        internal override int VehicleCount { get; } = 4;

        /// <summary>
        /// Gets the Vehicle Capacity for all vehicles, <c>30</c>.
        /// </summary>
        /// <value>30</value>
        internal override long VehicleCap { get; } = 30;

        /// <summary>
        /// Gets the Maximum Slack, <c>30</c>.
        /// </summary>
        /// <value>30</value>
        internal override long? SlackMaximum { get; } = 30;

        /// <summary>
        /// Gets whether to Zero Accumulator, <c>false</c>.
        /// </summary>
        /// <value>false</value>
        internal override bool? ZeroAccumulator { get; } = default(bool);

        /// <summary>
        /// Gets or Sets the ExpectedTotalDistance, <c>71</c>.
        /// </summary>
        /// <value>71</value>
        internal override long ExpectedTotalDistance { get; set; } = 71;

        // TODO: TBD: we could potentially refactor this sort of initialization to the test fixtures themselves...
        // TODO: TBD: rather than do them here...
        /// <summary>
        /// Gets the ExpectedPaths.
        /// </summary>
        internal override IList<int[]> ExpectedPaths { get; set; } = Range(
            Range(0, 9, 14, 16, 0).ToArray()
            , Range(0, 7, 1, 4, 3, 0).ToArray()
            , Range(0, 12, 13, 15, 11, 0).ToArray()
            , Range(0, 5, 8, 6, 2, 10, 0).ToArray()).ToList();

        private IEnumerable<int?> _expectedRouteDistances;

        /// <summary>
        /// Gets the ExpectedRouteDistances.
        /// </summary>
        internal virtual IEnumerable<int?> ExpectedRouteDistances => this._expectedRouteDistances ?? (
            this._expectedRouteDistances = Range(0, this.VehicleCount).Select(_ => (int?)30).ToArray()
        );

        /// <summary>
        /// Gets the Vehicle Routing DistanceUnit, <c>&quot;min&quot;</c>
        /// for lack of a better definition.
        /// </summary>
        /// <value>min</value>
        internal override string DistanceUnit { get; } = "min";

        /// <inheritdoc/>
        protected override int?[,] MatrixValues { get; } = {
            { 0, 6, 9, 8, 7, 3, 6, 2, 3, 2, 6, 6, 4, 4, 5, 9, 7 }
            , { 6, 0, 8, 3, 2, 6, 8, 4, 8, 8, 13, 7, 5, 8, 12, 10, 14 }
            , { 9, 8, 0, 11, 10, 6, 3, 9, 5, 8, 4, 15, 14, 13, 9, 18, 9 }
            , { 8, 3, 11, 0, 1, 7, 10, 6, 10, 10, 14, 6, 7, 9, 14, 6, 16 }
            , { 7, 2, 10, 1, 0, 6, 9, 4, 8, 9, 13, 4, 6, 8, 12, 8, 14 }
            , { 3, 6, 6, 7, 6, 0, 2, 3, 2, 2, 7, 9, 7, 7, 6, 12, 8 }
            , { 6, 8, 3, 10, 9, 2, 0, 6, 2, 5, 4, 12, 10, 10, 6, 15, 5 }
            , { 2, 4, 9, 6, 4, 3, 6, 0, 4, 4, 8, 5, 4, 3, 7, 8, 10 }
            , { 3, 8, 5, 10, 8, 2, 2, 4, 0, 3, 4, 9, 8, 7, 3, 13, 6 }
            , { 2, 8, 8, 10, 9, 2, 5, 4, 3, 0, 4, 6, 5, 4, 3, 9, 5 }
            , { 6, 13, 4, 14, 13, 7, 4, 8, 4, 4, 0, 10, 9, 8, 4, 13, 4 }
            , { 6, 7, 15, 6, 4, 9, 12, 5, 9, 6, 10, 0, 1, 3, 7, 3, 10 }
            , { 4, 5, 14, 7, 6, 7, 10, 4, 8, 5, 9, 1, 0, 2, 6, 4, 8 }
            , { 4, 8, 13, 9, 8, 7, 10, 3, 7, 4, 8, 3, 2, 0, 4, 5, 6 }
            , { 5, 12, 9, 14, 12, 6, 6, 7, 3, 3, 4, 7, 6, 4, 0, 9, 2 }
            , { 9, 10, 18, 6, 8, 12, 15, 8, 13, 9, 13, 3, 4, 5, 9, 0, 9 }
            , { 7, 14, 9, 16, 14, 8, 5, 10, 6, 5, 4, 10, 8, 6, 2, 9, 0 }
        };

        /// <summary>
        /// Gets the TimeWindows for use with the Model. The First window is that of the Depot.
        /// </summary>
        internal (long start, long end)[] TimeWindows { get; } = {
            (0, 5)
            , (7, 12)
            , (10, 15)
            , (16, 18)
            , (10, 13)
            , (0, 5)
            , (5, 10)
            , (0, 4)
            , (5, 10)
            , (0, 3)
            , (10, 16)
            , (10, 15)
            , (0, 5)
            , (5, 10)
            , (7, 8)
            , (10, 15)
            , (11, 15)
        };

        /// <inheritdoc/>
        protected override DistanceMatrix CreateMatrix() => new DistanceMatrix(this.MatrixValues);

        /// <inheritdoc/>
        protected override RoutingContext CreateContext() => new RoutingContext(this.Matrix.Length, this.VehicleCount, this.Depot);

        /// <inheritdoc/>
        public TimeWindowCaseStudyScope(ITestOutputHelper outputHelper)
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
