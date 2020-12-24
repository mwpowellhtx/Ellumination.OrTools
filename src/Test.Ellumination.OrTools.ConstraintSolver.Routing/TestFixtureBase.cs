using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;
    using static String;

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
        /// Internal adapter for the <see cref="Enumerable.Range"/> method.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static IEnumerable<int> Range(int start, int count) =>
            Enumerable.Range(start, count);

        /// <summary>
        /// Internal adapter for the <see cref="Enumerable.Repeat{TResult}"/> method.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="element"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static IEnumerable<TResult> Repeat<TResult>(TResult element, int count) =>
            Enumerable.Repeat(element, count);

        /// <summary>
        /// Returns the Single <typeparamref name="T"/> <paramref name="value"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static T Single<T>(T value) => Range(value).Single();

        /// <summary>
        /// Returns the Range of <paramref name="values"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        internal static IEnumerable<T> Range<T>(params T[] values)
        {
            foreach (var value in values)
            {
                yield return value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        internal static string Render<T>(T value, Func<T, string> callback = null)
        {
            callback = callback ?? (x => $"{x}");
            return callback.Invoke(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string Render(bool value) => Render(value, x => $"{x}".ToLower());

        /// <summary>
        /// Renders the Range of <paramref name="values"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        internal static string Render<T>(IEnumerable<T> values)
        {
            const char comma = ',';
            const string squareBrackets = "[]";
            string RenderRangeValue(T value) => value is bool b ? Render(b) : Render(value);
            return Join(Join($"{comma} ", values.Select(RenderRangeValue)), squareBrackets.ToArray());
        }

        /// <summary>
        /// Renders the Range of <paramref name="values"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        internal static string Render<T>(T[] values) => Render((IEnumerable<T>)values);

        /// <summary>
        /// Renders the Associate <paramref name="pairs"/>.
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        internal static IEnumerable<string> RenderAssociates(params (string name, string value)[] pairs)
        {
            foreach (var (name, value) in pairs)
            {
                const char colon = ':';
                yield return Join($"{colon} ", name, value);
            }
        }

        /// <summary>
        /// Renders the Associate <paramref name="pairs"/> as a Tuple.
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        internal static string RenderTupleAssociates(params (string name, string value)[] pairs)
        {
            const char comma = ',';
            return RenderTupleAssociates($"{comma} ", pairs);
        }

        /// <summary>
        /// Renders the Associate <paramref name="pairs"/> as a Tuple.
        /// </summary>
        /// <param name="delim"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        internal static string RenderTupleAssociates(string delim, params (string name, string value)[] pairs)
        {
            var rendered = RenderAssociates(pairs);
            const string curlyBraces = "{}";
            return Join(Join(delim, rendered), curlyBraces.ToArray());
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
