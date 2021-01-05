using System;
using System.Collections.Generic;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using TBridge = DurationBridge;
    using TBravo = Google.Protobuf.WellKnownTypes.Duration;

    /// <summary>
    /// Unlike other Bridges, there are no <see cref="Pairs"/> to maintain in this case,
    /// per se. Rather, we support converting between <see cref="TBravo"/> and its constitient
    /// <see cref="TBravo.Seconds"/> and <see cref="TBravo.Nanos"/> components, tuples, and so on.
    /// This Bridge implementation is a little bit tricky, because we must also allow for the
    /// possibility for the duration to be <c>null</c>, so the tuple by definition, or its
    /// constituents by extension, must also be <see cref="Nullable{T}"/>. Additionally, we
    /// allow for the conversions to and from its constituent <see cref="TBravo.Seconds"/>
    /// or <see cref="TBravo.Nanos"/> parts.
    /// </summary>
    public class DurationBridge : ValueBridge<(long seconds, int nanoseconds)?, TBravo, TBridge>
    {
        internal static (long seconds, int nanoseconds)? Null => null;

        /// <summary>
        /// We do not utilize Pairs in the sense that it was originally intended.
        /// </summary>
        protected override IEnumerable<((long seconds, int nanoseconds)? alpha, TBravo bravo)> Pairs =>
            throw new NotImplementedException();

        /// <inheritdoc/>
        protected override TBravo ConvertValue((long seconds, int nanoseconds)? value) => value == null
            ? null
            : new TBravo { Seconds = value?.seconds ?? default, Nanos = value?.nanoseconds ?? default };

        /// <inheritdoc/>
        protected override (long seconds, int nanoseconds)? ConvertValue(TBravo value) => value == null
            ? Null
            : (value.Seconds, value.Nanos);

        /// <summary>
        /// Constructs a new Bridge instance given <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        public DurationBridge((long seconds, int nanoseconds)? value) : base(value) { }

        /// <summary>
        /// Constructs a new Bridge instance given <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        public DurationBridge(TBravo value) : base(value) { }

        /// <summary>
        /// Implicitly Converts the <paramref name="seconds"/> to a <see cref="TBridge"/> instance.
        /// </summary>
        /// <param name="seconds"></param>
        public static implicit operator TBridge(long seconds) => (seconds, nanoseconds: 0);

        /// <summary>
        /// Implicitly Converts the <paramref name="seconds"/> to a <see cref="TBridge"/> instance.
        /// </summary>
        /// <param name="seconds"></param>
        public static implicit operator TBridge(long? seconds) => seconds == null
            ? Null
            : ((long seconds, int nanoseconds)?)(seconds, nanoseconds: 0);

        /// <summary>
        /// Implicitly Converts the <paramref name="value"/> to a Bridge instance.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator TBridge((long seconds, int nanoseconds)? value) => new TBridge(value);

        /// <summary>
        /// Implicitly Converts the <paramref name="value"/> to a Bridge instance.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator TBridge(TBravo value) => new TBridge(value);

        /// <summary>
        /// Implicitly extrapolates the bridged <see cref="TBravo.Seconds"/> component.
        /// </summary>
        /// <param name="bridge"></param>
        public static implicit operator long(TBridge bridge) => bridge.Bravo?.Seconds ?? default;

        /// <summary>
        /// Implicitly extrapolates the bridged <see cref="TBravo.Nanos"/> component.
        /// </summary>
        /// <param name="bridge"></param>
        public static implicit operator int(TBridge bridge) => bridge.Bravo?.Nanos ?? default;

        /// <summary>
        /// Implicitly extrapolates the bridged <see cref="TBravo.Seconds"/> component.
        /// </summary>
        /// <param name="bridge"></param>
        public static implicit operator long?(TBridge bridge) => bridge.Bravo?.Seconds;

        /// <summary>
        /// Implicitly extrapolates the bridged <see cref="TBravo.Seconds"/> component.
        /// </summary>
        /// <param name="bridge"></param>
        public static implicit operator int?(TBridge bridge) => bridge.Bravo?.Nanos;
    }
}
