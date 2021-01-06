using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools
{
    // TODO: TBD: short of adopting a "lite" dependency on something like an Automapper...
    /// <summary>
    /// Override in order to declare a type specific Bridge between two Values of Type
    /// <typeparamref name="TAlpha"/> and <typeparamref name="TBravo"/>, respectively.
    /// </summary>
    /// <typeparam name="TAlpha"></typeparam>
    /// <typeparam name="TBravo"></typeparam>
    /// <typeparam name="TBridge"></typeparam>
    /// <see cref="ConvertImplicitAlpha(TBridge)"/>
    /// <see cref="ConvertImplicitBravo(TBridge)"/>
    public abstract class ValueBridge<TAlpha, TBravo, TBridge>
        where TBridge : ValueBridge<TAlpha, TBravo, TBridge>
    {
        /// <summary>
        /// Gets the <typeparamref name="TAlpha"/> <see cref="Type"/>.
        /// </summary>
        protected static Type AlphaType { get; } = typeof(TAlpha);

        /// <summary>
        /// Gets the <typeparamref name="TBravo"/> <see cref="Type"/>.
        /// </summary>
        protected static Type BravoType { get; } = typeof(TBravo);

        private TAlpha _alpha;
        private TBravo _bravo;

        /// <summary>
        /// Override in order to specify the <typeparamref name="TAlpha"/> and
        /// <typeparamref name="TBravo"/> Pairs. Pairs is useful when we are bridging finite sets
        /// of values, enumerated values, and so forth. When it is unnecessary to do so, simply
        /// leave the enumerated Pairs unimplemented.
        /// </summary>
        /// <see cref="ExceptionExtensionMethods.ThrowException{TException}(object, object[])"/>
        protected virtual IEnumerable<(TAlpha alpha, TBravo bravo)> Pairs =>
            throw this.ThrowException<NotImplementedException>();

        /// <summary>
        /// Returns the Item associated with <paramref name="getter"/> after
        /// <paramref name="selector"/> identifies the Pair.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        protected virtual T GetPairItem<T>(Func<(TAlpha alpha, TBravo bravo), bool> selector
            , Func<(TAlpha alpha, TBravo bravo), T> getter) =>
            getter.Invoke(this.Pairs.Single(selector));

        /// <summary>
        /// Override in order to Convert <paramref name="value"/> to <typeparamref name="TAlpha"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract TAlpha ConvertValue(TBravo value);

        /// <summary>
        /// Override in order to Convert <paramref name="value"/> to <typeparamref name="TBravo"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract TBravo ConvertValue(TAlpha value);

        /// <summary>
        /// Gets or Sets the <typeparamref name="TAlpha"/> Value.
        /// </summary>
        public virtual TAlpha Alpha
        {
            get => this._alpha;
            set => this._bravo = this.ConvertValue(this._alpha = value);
        }

        /// <summary>
        /// Gets or Sets the <typeparamref name="TBravo"/> Value.
        /// </summary>
        public virtual TBravo Bravo
        {
            get => this._bravo;
            set => this._alpha = this.ConvertValue(this._bravo = value);
        }

        /// <summary>
        /// Constructs a new instance of the ValueBridge.
        /// </summary>
        /// <param name="value"></param>
        internal ValueBridge(TAlpha value) => this.Alpha = value;

        /// <summary>
        /// Constructs a new instance of the ValueBridge.
        /// </summary>
        /// <param name="value"></param>
        internal ValueBridge(TBravo value) => this.Bravo = value;

        /// <summary>
        /// Provides support for the <typeparamref name="TBridge"/> Implicit
        /// <typeparamref name="TAlpha"/> conversion.
        /// </summary>
        /// <param name="bridge"></param>
        /// <returns></returns>
        private static TAlpha ConvertImplicitAlpha(TBridge bridge) => bridge == null ? default : bridge.Alpha;

        /// <summary>
        /// Provides support for the <typeparamref name="TBridge"/> Implicit
        /// <typeparamref name="TBravo"/> conversion.
        /// </summary>
        /// <param name="bridge"></param>
        /// <returns></returns>
        private static TBravo ConvertImplicitBravo(TBridge bridge) => bridge == null ? default : bridge.Bravo;

        /// <summary>
        /// Implicitly converts <paramref name="bridge"/> to <typeparamref name="TAlpha"/>.
        /// </summary>
        /// <param name="bridge"></param>
        /// <remarks>For convenience, we provide the implicit conversion from the Bridge to
        /// the <typeparamref name="TAlpha"/> type. However, it is necessary for the adopting
        /// developer to provide his or her own <em>implicit</em> or <em>explicit</em> conversion,
        /// as the case may be, in the other direction, due to language constraints. We recommend
        /// <em>implicit</em>, of course.</remarks>
        /// <see cref="ConvertImplicitAlpha(TBridge)"/>
        public static implicit operator TAlpha(ValueBridge<TAlpha, TBravo, TBridge> bridge) => ConvertImplicitAlpha(bridge as TBridge);

        /// <summary>
        /// Implicitly converts <paramref name="bridge"/> to <typeparamref name="TBravo"/>.
        /// </summary>
        /// <param name="bridge"></param>
        /// <remarks>For convenience, we provide the implicit conversion from the Bridge to
        /// the <typeparamref name="TBravo"/> type. However, it is necessary for the adopting
        /// developer to provide his or her own <em>implicit</em> or <em>explicit</em> conversion,
        /// as the case may be, in the other direction, due to language constraints. We recommend
        /// <em>implicit</em>, of course.</remarks>
        /// <see cref="ConvertImplicitBravo(TBridge)"/>
        public static implicit operator TBravo(ValueBridge<TAlpha, TBravo, TBridge> bridge) => ConvertImplicitBravo(bridge as TBridge);

        /// <summary>
        /// Gets the <typeparamref name="TBridge"/> <see cref="Type"/>.
        /// </summary>
        protected static Type BridgeType { get; } = typeof(TBridge);
    }
}
