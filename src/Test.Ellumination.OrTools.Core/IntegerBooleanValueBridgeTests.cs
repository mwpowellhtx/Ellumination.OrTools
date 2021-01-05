using System;

namespace Ellumination.OrTools
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;
    using TBridge = IntegerBooleanValueBridgeTests.IntegerBooleanValueBridge;

    /// <summary>
    /// Performs <see cref="ValueBridge{TAlpha, TBravo, TBridge}"/> verification.
    /// </summary>
    public class IntegerBooleanValueBridgeTests : ValueBridgeTests<int, bool, TBridge>
    {
        public IntegerBooleanValueBridgeTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Bridges can bridge anything Alpha to and from Bravo. For simplicity we will
        /// Bridge <see cref="int"/> to and from <see cref="bool"/>. This should be more
        /// than sufficient to the task of performing Bridge unit tests.
        /// </summary>
        public class IntegerBooleanValueBridge : ValueBridge<int, bool, TBridge>
        {
            /// <inheritdoc/>
            protected override int ConvertValue(bool value) => value ? 1 : default;

            /// <inheritdoc/>
            protected override bool ConvertValue(int value) => value != default;

            /// <summary>
            /// Allows for verification that <see cref="Pairs"/> is not implemented.
            /// </summary>
            /// <returns>An <see cref="Action{T}"/> that may be verified.</returns>
            private Action OnVerifyPairs() => () => this.Pairs.AssertNull();

            /// <summary>
            /// Verifies when the Instance is Constructed.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="actual"></param>
            /// <param name="expected"></param>
            private void OnConstructed<T>(T actual, T expected)
            {
                this.OnVerifyPairs().AssertThrows<NotImplementedException>();
                actual.AssertEqual(expected);
            }

            /// <summary>
            /// Verifies when the Instance is Constructed.
            /// </summary>
            /// <param name="expected"></param>
            private void OnConstructed(int expected) => this.OnConstructed<int>(this, expected);

            /// <summary>
            /// Verifies when the Instance is Constructed.
            /// </summary>
            /// <param name="expected"></param>
            private void OnConstructed(bool expected) => this.OnConstructed<bool>(this, expected);

#pragma warning disable IDE0051 // Private member is unused
            /// <summary>
            /// Private Constructor.
            /// </summary>
            /// <param name="value"></param>
            private IntegerBooleanValueBridge(int value) : base(value) => this.OnConstructed(value);

            /// <summary>
            /// Private Constructor.
            /// </summary>
            /// <param name="value"></param>
            private IntegerBooleanValueBridge(bool value) : base(value) => this.OnConstructed(value);

            /// <summary>
            /// Implicitly converts from <paramref name="value"/> to a Bridge instance.
            /// </summary>
            /// <param name="value"></param>
            public static implicit operator TBridge(int value) => new TBridge(value);

            /// <summary>
            /// Implicitly converts from <paramref name="value"/> to a Bridge instance.
            /// </summary>
            /// <param name="value"></param>
            public static implicit operator TBridge(bool value) => new TBridge(value);
        }

        // TODO: TBD: may not require normalization after all...
        /// <inheritdoc/>
        protected override int NormalizeExpectedAlpha(int value) => value == default ? default : 1;

#pragma warning disable xUnit1008 // Test data attribute should only be used on a Theory (or Scenario)
        /// <inheritdoc/>
        [
            Example(0, false)
            , Example(1, true)
            , Example(-1, true)
            , Example(2, true)
        ]
        public override void Bridge_Conversions_Correct(int alpha, bool bravo)
        {
            // Assuming compilation succeeds, then we should also be able to verify.
            void OnVerifyAlpha(TBridge instance) => this.VerifyAlphaValue(instance, alpha);

            void OnVerifyBravo(TBridge instance) => this.VerifyBravoValue(instance, bravo);

            // First, we should observe compile time verification.
            $"Arrange for {nameof(alpha)} verification".x(() => OnBridgeInstance(alpha, OnVerifyAlpha));

            $"Arrange for {nameof(bravo)} verification".x(() => OnBridgeInstance(bravo, OnVerifyBravo));
        }
    }
}
