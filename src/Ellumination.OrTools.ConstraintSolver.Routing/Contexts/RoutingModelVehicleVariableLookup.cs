namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using IntVar = Google.OrTools.ConstraintSolver.IntVar;
    using RoutingModel = Google.OrTools.ConstraintSolver.RoutingModel;

    /// <summary>
    /// Provides an easy to use scaffold layer <see cref="RoutingModel.VehicleVar"/>
    /// interface to the rest of the architecture.
    /// </summary>
    internal class RoutingModelVehicleVariableLookup : VariableLookup<RoutingModel>, IRoutingModelVehicleVariableLookup
    {
        /// <summary>
        /// Constructs a <see cref="RoutingModel"/> Venicle Variable Lookup sourced
        /// by the <paramref name="model"/>.
        /// </summary>
        /// <param name="model"></param>
        internal RoutingModelVehicleVariableLookup(RoutingModel model)
            : base(model)
        {
        }

        /// <inheritdoc/>
        protected override IntVar Get(RoutingModel model, long index) => model.VehicleVar(index);

        /// <summary>
        /// Implicitly converts the <paramref name="model"/> to an instance
        /// of <see cref="RoutingModelVehicleVariableLookup"/>.
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator RoutingModelVehicleVariableLookup(RoutingModel model) =>
            new RoutingModelVehicleVariableLookup(model);
    }
}
