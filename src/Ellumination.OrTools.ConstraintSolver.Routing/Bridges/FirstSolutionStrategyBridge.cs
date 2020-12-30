using System.Collections.Generic;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    // TODO: TBD: again, much of which seems VERY boilerplate to us...
    // TODO: TBD: could we perhaps plan on a CG approach based on the verbose enum paths?
    // TODO: TBD: the goal being to simplify the namespaces involved, allowing for faster time to adoption...
    using TBridge = FirstSolutionStrategyBridge;
    using TAlpha = FirstSolutionStrategy;
    using TBravo = Google.OrTools.ConstraintSolver.FirstSolutionStrategy.Types.Value;

    /// <inheritdoc/>
    public class FirstSolutionStrategyBridge : ValueBridge<TAlpha, TBravo, TBridge>
    {
        /// <inheritdoc/>
        protected override IEnumerable<(TAlpha alpha, TBravo bravo)> Pairs { get; } = new[]
        {
            (TAlpha.Unset, TBravo.Unset)
            , (TAlpha.GlobalCheapestArc, TBravo.GlobalCheapestArc)
            , (TAlpha.LocalCheapestArc, TBravo.LocalCheapestArc)
            , (TAlpha.PathCheapestArc, TBravo.PathCheapestArc)
            , (TAlpha.PathMostConstrainedArc, TBravo.PathMostConstrainedArc)
            , (TAlpha.EvaluatorStrategy, TBravo.EvaluatorStrategy)
            , (TAlpha.AllUnperformed, TBravo.AllUnperformed)
            , (TAlpha.BestInsertion, TBravo.BestInsertion)
            , (TAlpha.ParallelCheapestInsertion, TBravo.ParallelCheapestInsertion)
            , (TAlpha.LocalCheapestInsertion, TBravo.LocalCheapestInsertion)
            , (TAlpha.Savings, TBravo.Savings)
            , (TAlpha.Sweep, TBravo.Sweep)
            , (TAlpha.FirstUnboundMinValue, TBravo.FirstUnboundMinValue)
            , (TAlpha.Christofides, TBravo.Christofides)
            , (TAlpha.SequentialCheapestInsertion, TBravo.SequentialCheapestInsertion)
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
        public FirstSolutionStrategyBridge(TAlpha value) : base(value) { }

        /// <summary>
        /// Constructs a new Bridge instance given <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        public FirstSolutionStrategyBridge(TBravo value) : base(value) { }

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
