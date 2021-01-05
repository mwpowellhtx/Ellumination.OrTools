using System.Collections.Generic;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using TBridge = OptionalBooleanBridge;
    using TAlpha = System.Nullable<bool>;
    using TBravo = Google.OrTools.Util.OptionalBoolean;

    /// <inheritdoc/>
    public class OptionalBooleanBridge : ValueBridge<TAlpha, TBravo, TBridge>
    {
        /// <summary>
        /// Gets the Null <see cref="TAlpha"/> Value.
        /// </summary>
        internal static TAlpha Null { get; } = null;

        /// <inheritdoc/>
        protected override IEnumerable<(TAlpha alpha, TBravo bravo)> Pairs { get; } = new[]
        {
            (Null, TBravo.BoolUnspecified)
            , (false, TBravo.BoolFalse)
            , (true, TBravo.BoolTrue)
        };

        /// <inheritdoc/>
        protected override TBravo ConvertValue(TAlpha value) => this.GetPairItem(p => p.alpha == value, p => p.bravo);

        /// <inheritdoc/>
        protected override TAlpha ConvertValue(TBravo value) => this.GetPairItem(p => p.bravo == value, p => p.alpha);

        /// <summary>
        /// Constructs a new Bridge instance given <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        public OptionalBooleanBridge(TAlpha value) : base(value) { }

        /// <summary>
        /// Constructs a new Bridge instance given <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        public OptionalBooleanBridge(TBravo value) : base(value) { }

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
