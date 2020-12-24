using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using static TestFixtureBase;

    /// <summary>
    /// 
    /// </summary>
    public class TimeWindowResourceConstraintDimension : TimeWindowDimension
    {
        /// <summary>
        /// Constructs the dimension.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scope"></param>
        public TimeWindowResourceConstraintDimension(RoutingContext context
            , TimeWindowResourceConstraintCaseStudyScope scope)
            : base(context, scope)
        {
            if (this.Coefficient > 0)
            {
                // TODO: TBD: may need access to the registered evaluator index...
                this.OnArrangeVehicleDeliveryIntervals(scope);
                this.OnArrangeDepotUsage(scope);
            }
        }

        /// <summary>
        /// &quot;depot_interval&quot;
        /// </summary>
        private const string depot_interval = nameof(depot_interval);

        // TODO: TBD: in the example, start/load, end/unload is the pairing...
        // TODO: TBD: however, from a logistical perspective, we think that should be reversed...
        // TODO: TBD: i.e. vehicles unload at start, and load prior to end...

        // TODO: TBD: this is how it works for the example...
        // TODO: TBD: however we think that the interval could even perhaps vary from vehicle to vehicle, or possibly depot to depot?
        // TODO: TBD: although the approach seems to us to be a bit of a kluge...

        /// <summary>
        /// Arranges the Vehicle Delivery Intervals.
        /// </summary>
        /// <param name="scope"></param>
        /// <see cref="!:https://developers.google.com/optimization/routing/cvrptw_resources#loading_constraints"/>
        private void OnArrangeVehicleDeliveryIntervals(TimeWindowResourceConstraintCaseStudyScope scope) => this.AddFixedDurationIntervals(
            (scope.VehicleLoadTime, scope.VehicleUnloadTime), depot_interval, Range(0, scope.VehicleCount).ToArray()
        );

        /// <summary>
        /// &quot;depot&quot;
        /// </summary>
        private const string depot = nameof(depot);

        /// <summary>
        /// Arranges for Depot Resource interval usage.
        /// </summary>
        /// <param name="scope"></param>
        private void OnArrangeDepotUsage(TimeWindowResourceConstraintCaseStudyScope scope) =>
            this.AddResourceIntervalUsageConstraint(depot_interval, scope.DepotCap, depot);
    }
}
