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
        public virtual IEnumerable<long> Demands { get; } = Range<long>(
            0, 1, 1, 2, 4, 2, 4, 8, 8, 1, 2, 1, 2, 4, 4, 8, 8).ToArray();

        private IEnumerable<long> _vehicleCaps;

        /// <summary>
        /// Gets the VehicleCaps informing the Case Study tests.
        /// </summary>
        internal virtual IEnumerable<long> VehicleCaps => this._vehicleCaps ?? (
            this._vehicleCaps = Range(0, this.VehicleCount).Select(_ => 15L).ToArray()
        );

        /// <inheritdoc/>
        internal override IList<int[]> ExpectedPaths { get; set; } = Range(
            Range(0, 1, 4, 3, 15, 0).ToArray()
            , Range(0, 14, 16, 10, 2, 0).ToArray()
            , Range(0, 7, 13, 12, 11, 0).ToArray()
            , Range(0, 9, 8, 6, 5, 0).ToArray()).ToList();

        /// <inheritdoc/>
        internal override IEnumerable<int?> ExpectedRouteDistances { get; } = Range<int?>(2192, 2192, 1324, 1164);

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
        /// Gets the Vehicle Routing DistanceUnit, <c>&quot;units&quot;</c>
        /// for lack of a better definition.
        /// </summary>
        /// <value>m</value>
        internal override string DistanceUnit { get; } = "m";
    }
}
