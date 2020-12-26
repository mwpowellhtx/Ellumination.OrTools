using System.Collections.Generic;
using System.Linq;

#if false

Objective: 7936

Dropped nodes: 6 15

Route for vehicle 0:
 0 Load(0) ->  9 Load(1) ->  14 Load(7) ->  16 Load(15) ->  0 Load(15)
Distance of the route: 1324m
Load of the route: 15

Route for vehicle 1:
 0 Load(0) ->  12 Load(2) ->  11 Load(3) ->  4 Load(9) ->  3 Load(12) ->  1 Load(13) ->  0 Load(13)
Distance of the route: 1872m
Load of the route: 13

Route for vehicle 2:
 0 Load(0) ->  7 Load(8) ->  13 Load(14) ->  0 Load(14)
Distance of the route: 868m
Load of the route: 14

Route for vehicle 3:
 0 Load(0) ->  8 Load(8) ->  10 Load(10) ->  2 Load(11) ->  5 Load(14) ->  0 Load(14)
Distance of the route: 1872m
Load of the route: 14

Total Distance of all routes: 5936m

Total Load of all routes: 56

#endif // false

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using Xunit;
    using Xunit.Abstractions;
    using static TestFixtureBase;

    /// <inheritdoc/>
    public class PenaltyConstraintVehicleRoutingCaseStudyScope : ConstraintVehicleRoutingCaseStudyScope
    {
        /// <summary>
        /// Gets the Penalty associated with the Scope, for use with Disjunctions.
        /// Defaults to <c>1000</c>.
        /// </summary>
        /// <value>1000</value>
        /// <see cref="!:https://developers.google.com/optimization/routing/penalties#add-the-capacity-constraints-and-penalties"/>
        internal override long? Penalty { get; } = 1000;

        /// <summary>
        /// Gets the Demands informing the Case Study tests.
        /// </summary>
        /// <see cref="!:https://developers.google.com/optimization/routing/penalties#create-the-data"/>
        public override IEnumerable<long> Demands { get; } = Range<long>(
            0, 1, 1, 3, 6, 3, 6, 8, 8, 1, 2, 1, 2, 6, 6, 8, 8).ToArray();

        /// <inheritdoc/>
        internal override IList<int[]> ExpectedPaths { get; set; } = Range(
            Range(0, 9, 14, 16, 0).ToArray()
            , Range(0, 12, 11, 4, 3, 1, 0).ToArray()
            , Range(0, 7, 13, 0).ToArray()
            , Range(0, 8, 10, 2, 5, 0).ToArray()).ToList();

        /// <summary>
        /// Gets the ExpectedRouteDistances.
        /// </summary>
        /// <see cref="!:https://developers.google.com/optimization/routing/penalties#running-the-program"/>
        internal override IEnumerable<int?> ExpectedRouteDistances { get; } = Range<int?>(1324, 1872, 868, 5936);

        /// <summary>
        /// Constructs a Case Study Scope instance.
        /// </summary>
        /// <param name="outputHelper"></param>
        public PenaltyConstraintVehicleRoutingCaseStudyScope(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        // TODO: TBD: what is the proper distance unit?
        /// <summary>
        /// Gets the Vehicle Routing DistanceUnit, <c>&quot;m&quot;</c>
        /// for lack of a better definition.
        /// </summary>
        /// <value>m</value>
        internal override string DistanceUnit { get; } = "m";
    }
}
