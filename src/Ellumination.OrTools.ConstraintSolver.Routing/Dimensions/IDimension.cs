namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    /// <summary>
    /// Represents a simple <see cref="Dimension"/>.
    /// </summary>
    public interface IDimension
    {
        /// <summary>
        /// Gets an AccumulatorLookup instance corresponding to the Dimension.
        /// </summary>
        IRoutingDimensionAccumulatorLookup AccumulatorLookup { get; }
    }
}
