using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit.Abstractions;
    using static TestFixtureBase;

    /// <summary>
    /// Represents the Scope concerning <see cref="TimeWindowCaseStudyScope"/> for
    /// the Resource Constraint, or <em>CVRPTW</em>, problem.
    /// </summary>
    /// <see cref="!:https://developers.google.com/optimization/routing/cvrptw_resources#create-the-data"/>
    public class TimeWindowResourceConstraintCaseStudyScope : TimeWindowCaseStudyScope
    {
        /// <inheritdoc/>
        internal override IList<int[]> ExpectedPaths { get; set; } = Range(
            Range(0, 8, 14, 16, 0).ToArray()
            , Range(0, 12, 13, 15, 11, 0).ToArray()
            , Range(0, 7, 1, 4, 3, 0).ToArray()
            , Range(0, 9, 5, 6, 2, 10, 0).ToArray()).ToList();

        private IEnumerable<int?> _expectedRouteDistances;

        /// <summary>
        /// Gets the ExpectedRouteDistances.
        /// </summary>
        internal override IEnumerable<int?> ExpectedRouteDistances => this._expectedRouteDistances ?? (
            this._expectedRouteDistances = Range(0, this.VehicleCount).Select(_ => (int?)30).ToArray()
        );

        /// <summary>
        /// Redefine the TimeWindows aligned with the web site.
        /// </summary>
        /// <see cref="!:https://developers.google.com/optimization/routing/cvrptw_resources#vrptw-example-with-resource-constraints"/>
        internal override (long start, long end)[] TimeWindows { get; } = {
            /*    0 */ (0, 5)
            , /*  1 */ (7, 12)
            , /*  2 */ (10, 15)
            , /*  3 */ (5, 14)
            , /*  4 */ (5, 13)
            , /*  5 */ (0, 5)
            , /*  6 */ (5, 10)
            , /*  7 */ (0, 10)
            , /*  8 */ (5, 10)
            , /*  9 */ (0, 5)
            , /* 10 */ (10, 16)
            , /* 11 */ (10, 15)
            , /* 12 */ (0, 5)
            , /* 13 */ (5, 10)
            , /* 14 */ (7, 12)
            , /* 15 */ (10, 15)
            , /* 16 */ (5, 15)
        };

        /// <summary>
        /// Gets the VehicleLoadTime, <c>5</c>.
        /// </summary>
        /// <value>5</value>
        internal virtual long VehicleLoadTime { get; } = 5;

        /// <summary>
        /// Gets the VehicleUnloadTime, <c>5</c>.
        /// </summary>
        /// <value>5</value>
        internal virtual long VehicleUnloadTime { get; } = 5;

        /// <summary>
        /// Gets the DepotCap, capacity, <c>2</c>.
        /// </summary>
        /// <value>2</value>
        internal virtual int DepotCap { get; } = 2;

        /// <inheritdoc/>
        public TimeWindowResourceConstraintCaseStudyScope(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }
    }
}
