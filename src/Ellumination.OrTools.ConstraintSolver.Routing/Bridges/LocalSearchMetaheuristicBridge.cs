using System.Collections.Generic;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    // TODO: TBD: again, much of which seems VERY boilerplate to us...
    // TODO: TBD: could we perhaps plan on a CG approach based on the verbose enum paths?
    // TODO: TBD: the goal being to simplify the namespaces involved, allowing for faster time to adoption...
    using TBridge = LocalSearchMetaheuristicBridge;
    using TAlpha = LocalSearchMetaheuristic;
    using TBravo = Google.OrTools.ConstraintSolver.LocalSearchMetaheuristic.Types.Value;

    /// <inheritdoc/>
    public class LocalSearchMetaheuristicBridge : ValueBridge<TAlpha, TBravo, TBridge>
    {
        /// <inheritdoc/>
        protected override IEnumerable<(TAlpha alpha, TBravo bravo)> Pairs { get; } = new[]
        {
            (TAlpha.Unset, TBravo.Unset)
            , (TAlpha.GreedyDescent, TBravo.GreedyDescent)
            , (TAlpha.GuidedLocalSearch, TBravo.GuidedLocalSearch)
            , (TAlpha.SimulatedAnnealing, TBravo.SimulatedAnnealing)
            , (TAlpha.TabuSearch, TBravo.TabuSearch)
            , (TAlpha.GenericTabuSearch, TBravo.GenericTabuSearch)
            , (TAlpha.Automatic, TBravo.Automatic)
        };

        /// <inheritdoc/>
        protected override TBravo ConvertValue(TAlpha value) => this.GetPairItem(p => p.alpha == value, p => p.bravo);

        /// <inheritdoc/>
        protected override TAlpha ConvertValue(TBravo value) => this.GetPairItem(p => p.bravo == value, p => p.alpha);

        /// <summary>
        /// Constructs a new Bridge instance given <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        public LocalSearchMetaheuristicBridge(TAlpha value) : base(value) { }

        /// <summary>
        /// Constructs a new Bridge instance given <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        public LocalSearchMetaheuristicBridge(TBravo value) : base(value) { }

        /// <summary>
        /// Implicitly Converts the <paramref name="value"/> to a Bridge instance.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator TBridge(TAlpha value) => new TBridge(value);

        /// <summary>
        /// Implicitly Converts the <paramref name="value"/> to a Bridge instance.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator TBridge(TBravo value) => new TBridge(value);
    }
}
