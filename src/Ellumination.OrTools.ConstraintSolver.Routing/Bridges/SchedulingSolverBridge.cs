using System.Collections.Generic;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using TBridge = SchedulingSolverBridge;
    using TAlpha = SchedulingSolver;
    using TBravo = Google.OrTools.ConstraintSolver.RoutingSearchParameters.Types.SchedulingSolver;

    /// <inheritdoc/>
    public class SchedulingSolverBridge : ValueBridge<TAlpha, TBravo, TBridge>
    {
        /// <inheritdoc/>
        protected override IEnumerable<(TAlpha alpha, TBravo bravo)> Pairs { get; } = new[]
        {
            (TAlpha.Unset, TBravo.Unset)
            , (TAlpha.Glop, TBravo.Glop)
            , (TAlpha.CpSat, TBravo.CpSat)
        };

        /// <inheritdoc/>
        protected override TBravo ConvertValue(TAlpha value) => this.GetPairItem(p => p.alpha == value, p => p.bravo);

        /// <inheritdoc/>
        protected override TAlpha ConvertValue(TBravo value) => this.GetPairItem(p => p.bravo == value, p => p.alpha);

        /// <summary>
        /// Constructs a new Bridge instance given <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        public SchedulingSolverBridge(TAlpha value) : base(value) { }

        /// <summary>
        /// Constructs a new Bridge instance given <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        public SchedulingSolverBridge(TBravo value) : base(value) { }

        /// <summary>
        /// Implicitly Converts the <paramref name="value"/> to a Bridge instance.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator TBridge(TAlpha value) => CreateBridge<TBridge>(value);

        /// <summary>
        /// Implicitly Converts the <paramref name="value"/> to a Bridge instance.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator TBridge(TBravo value) => CreateBridge<TBridge>(value);
    }
}
