using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;

    /// <summary>
    /// Test fixture base class.
    /// </summary>
    public abstract class TestFixtureBase : IDisposable
    {
        protected ITestOutputHelper OutputHelper { get; }

        /// <summary>
        /// Protected constructor.
        /// </summary>
        /// <param name="outputHelper"></param>
        protected TestFixtureBase(ITestOutputHelper outputHelper)
        {
            this.OutputHelper = outputHelper;
        }

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
        /// Decouples the <paramref name="ep"/> Endpoint. We leverage the
        /// refactored extension method, expose it within for convenience.
        /// </summary>
        /// <param name="ep"></param>
        /// <returns></returns>
        protected static IEnumerable<int> OnDecoupleEndpoint((int start, int end) ep) => ep.DecoupleEndpoint();

        [Background]
        public void BaseBaseBackground()
        {
            "Initialize the base base class".x(() => true.AssertTrue());
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
