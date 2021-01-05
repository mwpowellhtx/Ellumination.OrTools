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
        /// Override in order to specify the <typeparamref name="TAlpha"/>
        /// and <typeparamref name="TBravo"/> Pairs.
        /// </summary>
        protected virtual IEnumerable<(TAlpha alpha, TBravo bravo)> Pairs => throw new NotImplementedException();

        /// <summary>
        /// Returns the Item associated with <paramref name="getter"/> after
        /// <paramref name="selector"/> identifies the Pair.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        protected virtual T GetPairItem<T>(Func<(TAlpha alpha, TBravo bravo), bool> selector, Func<(TAlpha alpha, TBravo bravo), T> getter) =>
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
        /// Implicitly converts <paramref name="bridge"/> to <typeparamref name="TAlpha"/>.
        /// </summary>
        /// <param name="bridge"></param>
        public static implicit operator TAlpha(ValueBridge<TAlpha, TBravo, TBridge> bridge) => bridge.Alpha;

        /// <summary>
        /// Implicitly converts <paramref name="bridge"/> to <typeparamref name="TBravo"/>.
        /// </summary>
        /// <param name="bridge"></param>
        public static implicit operator TBravo(ValueBridge<TAlpha, TBravo, TBridge> bridge) => bridge.Bravo;

        /// <summary>
        /// Gets the <typeparamref name="TBridge"/> <see cref="Type"/>.
        /// </summary>
        protected static Type BridgeType { get; } = typeof(TBridge);

        ///// <summary>
        ///// Returns a Created <typeparamref name="TBridge"/> given the <paramref name="value"/>.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="value"></param>
        ///// <returns>Created <typeparamref name="TBridge"/> given the <paramref name="value"/>.</returns>
        //protected static TBridge CreateBridge<T>(T value) => (TBridge)Activator.CreateInstance(BridgeType, value);
    }
}
