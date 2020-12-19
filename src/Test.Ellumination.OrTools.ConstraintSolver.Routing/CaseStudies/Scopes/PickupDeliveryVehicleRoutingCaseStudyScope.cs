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
        /// Gets the VehicleCap, <c>3000</c>.
        /// </summary>
        /// <value>3000</value>
        internal override long VehicleCap { get; } = 3000;

        /// <summary>
        /// Gets the Nodes From which Pickups must be made, paired with the Nodes To which
        /// Deliveries must be made. The pairings are an absolute requirement in the model,
        /// and that the Deliveries must occur after their respective Pickups, conversely
        /// that Pickups must occur sooner than their respective Deliveries.
        /// </summary>
        internal virtual IEnumerable<(int pickup, int delivery)> PickupDeliveries { get; } = Range(
            (1, 6)
            , (2, 10)
            , (4, 3)
            , (5, 9)
            , (7, 8)
            , (15, 11)
            , (13, 12)
            , (16, 14)
        ).ToArray();

        //private ICollection<int[]> _expectedPaths;

        /// <inheritdoc/>
        [Obsolete] // TODO: TBD: may not care about the expected paths in this instance...
        internal override IList<int[]> ExpectedPaths => throw new NotImplementedException();

        /// <inheritdoc/>
        internal override IEnumerable<int?> ExpectedRouteDistances { get; } = Range<int?>(1780, 1780, 2032, 1712);

        /// <inheritdoc/>
        public PickupDeliveryVehicleRoutingCaseStudyScope(ITestOutputHelper outputHelper)
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
        private static int Max(int val1, int val2) => Math.Max(val1, val2);
    }
}
