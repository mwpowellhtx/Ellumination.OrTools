using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;

    public abstract class TestFixtureBase : IDisposable
    {
        /// <summary>
        /// Returns the Range of <paramref name="values"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        protected static IEnumerable<T> Range<T>(params T[] values)
        {
            foreach (var value in values)
            {
                yield return value;
            }
        }

        /// <summary>
        /// Gets the OutputHelper.
        /// </summary>
        protected ITestOutputHelper OutputHelper { get; }

        /// <summary>
        /// Gets the Calculator for use throughout the fixture.
        /// </summary>
        protected virtual FibonacciCalculator Calculator { get; private set; }

        /// <summary>
        /// Constructs the fixture given parameters.
        /// </summary>
        /// <param name="outputHelper"></param>
        protected TestFixtureBase(ITestOutputHelper outputHelper)
        {
            this.OutputHelper = outputHelper;
        }

        /// <summary>
        /// Performs some Base Background initialization.
        /// </summary>
        [Background]
        public void BaseBackground()
        {
            $"Initializes the {nameof(Calculator)}".x(() => this.Calculator = new FibonacciCalculator());

            $"Verify the {nameof(Calculator)}".x(() => Calculator.AssertNotNull().AssertEqual(Range(0, 1)));
        }

        /// <summary>
        /// Gets whether IsDisposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            this.IsDisposed = true;
        }
    }
}
