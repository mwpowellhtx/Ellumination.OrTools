namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using IntVar = Google.OrTools.ConstraintSolver.IntVar;
    using RoutingDimension = Google.OrTools.ConstraintSolver.RoutingDimension;

    /// <summary>
    /// Provides an easy to use scaffold layer <see cref="RoutingDimension.CumulVar"/>
    /// interface to the rest of the architecture.
    /// </summary>
    public class RoutingDimensionAccumulatorLookup : VariableLookup<RoutingDimension>, IRoutingDimensionAccumulatorLookup
    {
        /// <summary>
        /// Constructs a Accumulator Lookup sourced by the <paramref name="rdim"/>.
        /// </summary>
        /// <param name="rdim"></param>
        internal RoutingDimensionAccumulatorLookup(RoutingDimension rdim)
            : base(rdim)
        {
        }

        /// <inheritdoc/>
        protected override IntVar Get(RoutingDimension rdim, long index) => rdim.CumulVar(index);

        /// <summary>
        /// Implicitly converts the <paramref name="rdim"/> to an instance
        /// of <see cref="RoutingDimensionAccumulatorLookup"/>.
        /// </summary>
        /// <param name="rdim"></param>
        public static implicit operator RoutingDimensionAccumulatorLookup(RoutingDimension rdim) =>
            new RoutingDimensionAccumulatorLookup(rdim);
    }
}
