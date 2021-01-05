using System;

namespace Ellumination.OrTools
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;

    /// <summary>
    /// Performs <typeparamref name="TBridge"/> verification.
    /// </summary>
    /// <typeparam name="TAlpha"></typeparam>
    /// <typeparam name="TBravo"></typeparam>
    /// <typeparam name="TBridge"></typeparam>
    public abstract class ValueBridgeTests<TAlpha, TBravo, TBridge> : TestFixtureBase
        where TBridge : ValueBridge<TAlpha, TBravo, TBridge>
    {
        protected ValueBridgeTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Gets or Sets the <typeparamref name="TBridge"/> Instance.
        /// </summary>
        protected TBridge Instance { get; set; }

        /// <summary>
        /// Performs a little Background scenario arrangement.
        /// </summary>
        [Background]
        public void ValueBridgeBackground()
        {
            $"Default {nameof(this.Instance)} is Null".x(() => this.Instance.AssertNull());
        }

        /// <summary>
        /// Verifies that the <paramref name="actual"/> <typeparamref name="T"/>
        /// value Equals <paramref name="expected"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bridge"></param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        protected virtual void VerifyValue<T>(TBridge bridge, T expected, Func<TBridge, T> actual) =>
            actual.Invoke(bridge.AssertNotNull()).AssertEqual(expected);

        /// <summary>
        /// Verifies that the <see cref="Instance"/> <typeparamref name="TBravo"/> value Equals
        /// <paramref name="expected"/>
        /// </summary>
        /// <param name="bridge"></param>
        /// <param name="expected"></param>
        protected virtual void VerifyAlphaValue(TBridge bridge, TAlpha expected) => this.VerifyValue(bridge, expected, _ => _);

        /// <summary>
        /// Verifies that the <see cref="Instance"/> <typeparamref name="TBravo"/> value Equals
        /// <paramref name="expected"/>
        /// </summary>
        /// <param name="bridge"></param>
        /// <param name="expected"></param>
        protected virtual void VerifyBravoValue(TBridge bridge, TBravo expected) => this.VerifyValue(bridge, expected, _ => _);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        protected virtual void OnBridgeInstance(TBridge instance, Action<TBridge> onVerify = null) =>
            onVerify?.Invoke(this.Instance = instance.AssertNotSame(this.Instance).AssertIsType<TBridge>());

        /// <summary>
        /// Normalizes the Expected <typeparamref name="TAlpha"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract TAlpha NormalizeExpectedAlpha(TAlpha value);

        /// <summary>
        /// Arranges for <typeparamref name="TBridge"/> Conversion verification.
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="bravo"></param>
        [Scenario]
        public abstract void Bridge_Conversions_Correct(TAlpha alpha, TBravo bravo);
    }
}
