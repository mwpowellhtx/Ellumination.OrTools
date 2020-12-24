using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.CaseStudies
{
    using DistanceMatrix = Distances.DistanceMatrix;
    using RoutingModel = Google.OrTools.ConstraintSolver.RoutingModel;

    /// <summary>
    /// Refactored the Dimension on account that we want to derive the class, and the generics
    /// are getting in the way a bit. So we can simplify that concern by refactoring.
    /// </summary>
    public class TimeWindowDimension : Dimension
    {
        /// <summary>
        /// Gets the Scope for Private use.
        /// </summary>
        protected TimeWindowCaseStudyScope Scope { get; }

        /// <summary>
        /// Gets the Matrix for Private use.
        /// </summary>
        private DistanceMatrix Matrix => this.Scope.Matrix;

        /// <summary>
        /// Constructs the dimension.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scope"></param>
        /// <see cref="!:https://developers.google.com/optimization/routing/tsp#distance_callback"/>
        /// <see cref="!:https://developers.google.com/optimization/routing/dimensions#slack_variables"/>
        /// <see cref="!:https://github.com/google/or-tools/discussions/2298">Concerning slack</see>
        public TimeWindowDimension(RoutingContext context, TimeWindowCaseStudyScope scope)
            : base(context, scope.DimensionCoefficient)
        {
            if (this.Coefficient > 0)
            {
                this.Scope = scope;

                var evaluatorIndex = this.RegisterTransitEvaluationCallback(this.OnEvaluateTransit);

                // TODO: TBD: I'm not sure we actually ever add the dimension here ...
                // TODO: TBD: should review the example again to make sure that we actually should/are/do... (take your pick)
                this.SetArcCostEvaluators(evaluatorIndex);

                // TODO: TBD: with parameters that could be captured in a context, etc...
                this.AddDimension(evaluatorIndex, scope.VehicleCap, scope.SlackMaximumOrDefault, scope.ZeroAccumulatorOrDefault);

                // Arrange the several constraints dealing with the vehicles, start, end, etc.
                this.OnArrangeVehicleTimeWindows(scope);
                this.OnArrangeVehicleStartTimeWindows(scope);
                this.OnArrangeVehicleStartEndFinalizers(scope);
            }
        }

        /// <summary>
        /// Add time window constraints for each location except depot.
        /// </summary>
        /// <param name="scope"></param>
        /// <see cref="!:https://developers.google.com/optimization/routing/vrptw#time_window_constraints"/>
        private void OnArrangeVehicleTimeWindows(TimeWindowCaseStudyScope scope)
        {
            var this_Context_DepotCoords = this.Context.DepotCoordinates.ToArray();

            // Instead of assuming "0" is the Depot, here we scan "around" the known Depot coordinates.
            bool DepotCoordinatesDoNotContain(int _) => !this_Context_DepotCoords.Contains(_);

            // TODO: TBD: we may want to follow a similar tract here re: RC versus RoutingIndexManager...
            // TODO: TBD: however we just prefer to abstract those sorts of bits through the RC...
            // TODO: TBD: if there is a case for it, we will, but for now, the interface is sufficient...
            void OnSetCumulVarTimeWindow(int node, in (long start, long end) timeWin) => this.SetCumulVarRange(
                node, timeWin, (RoutingContext _, int i) => _.NodeToIndex(i)
            );

            void OnSetCumulVarRange(int node) => OnSetCumulVarTimeWindow(
                node
                , scope.TimeWindows.ElementAt(node)
            );

            // Add Time Window constraints for each Node except the Depot(s).
            Enumerable.Range(default, this.Context.NodeCount)
                .Where(DepotCoordinatesDoNotContain)
                .ToList().ForEach(OnSetCumulVarRange);
        }

        /// <summary>
        /// Add time window constraints for each vehicle start node.
        /// </summary>
        /// <param name="scope"></param>
        /// <see cref="!:https://developers.google.com/optimization/routing/vrptw#time_window_constraints"/>
        private void OnArrangeVehicleStartTimeWindows(TimeWindowCaseStudyScope scope)
        {
            var firstTimeWindow = scope.TimeWindows.First();

            // Add time window constraints for each vehicle start node.
            void OnSetVehicleTimeWindow(int vehicle) => this.SetCumulVarRange(
                vehicle, firstTimeWindow, (RoutingModel model, int i) => model.Start(i)
            );

            var vehicles = Enumerable.Range(default, scope.VehicleCount).ToList();

            vehicles.ForEach(OnSetVehicleTimeWindow);
        }

        /// <summary>
        /// Arranges the Vehicle Start and End Finalizers.
        /// </summary>
        /// <param name="scope"></param>
        /// <see cref="!:https://developers.google.com/optimization/routing/vrptw#time_window_constraints"/>
        private void OnArrangeVehicleStartEndFinalizers(TimeWindowCaseStudyScope scope)
        {
            // Arrange for each Vehicle Start and End Finalizer concerns.
            void OnArrangeVehicleFinalizers(int vehicle)
            {
                // Illustrating how we can leverage either the root methods or extension methods.
                this.AddVariableMinimizedByFinalizer(vehicle, (RoutingContext _, int i) => _.Model.Start(i));
                this.AddVariableMinimizedByFinalizer(vehicle, (RoutingModel model, int i) => model.End(i));
            }

            var vehicles = Enumerable.Range(default, scope.VehicleCount).ToList();

            vehicles.ForEach(OnArrangeVehicleFinalizers);
        }

        /// <summary>
        /// Returns the result after Evaluating <see cref="Matrix"/> given
        /// <paramref name="fromNode"/> and <paramref name="toNode"/>.
        /// </summary>
        /// <param name="fromNode"></param>
        /// <param name="toNode"></param>
        /// <returns></returns>
        private long OnEvaluateTransit(int fromNode, int toNode) => this.Matrix[fromNode, toNode] ?? default;

        /// <summary>
        /// Returns the result after Evaluating <see cref="Matrix"/> given
        /// <paramref name="fromIndex"/> and <paramref name="toIndex"/>.
        /// </summary>
        /// <param name="fromIndex"></param>
        /// <param name="toIndex"></param>
        /// <returns></returns>
        /// <see cref="!:https://developers.google.com/optimization/routing/tsp#distance_callback"/>
        private long OnEvaluateTransit(long fromIndex, long toIndex) => this.OnEvaluateTransit(
            this.Context.IndexToNode(fromIndex), this.Context.IndexToNode(toIndex)
        );
    }
}
