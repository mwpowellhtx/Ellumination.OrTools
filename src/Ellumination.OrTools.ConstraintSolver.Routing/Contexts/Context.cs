using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing
{
    using Distances;
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// Represents the basic building blocks of a <see cref="RoutingModel"/> Context. While
    /// changes to the moving parts, Nodes or Vehicles, as the case may be, depending on the
    /// domain model, is possible, we advise against it in a given Context. If such mutations
    /// are desired, then create a new Context instance with those moving parts having been
    /// mutated.
    /// </summary>
    public abstract class Context : IDisposable
    {
        /// <summary>
        /// Gets or Sets the DistancesMatrix.
        /// </summary>
        /// <remarks><see cref="Context"/> must allow for an expression of the
        /// <see cref="DistanceMatrix"/>. However, we think that <see cref="Context"/>
        /// lacks sufficient context, all punning aside, in order to determine
        /// appropriate serialization, merging, updates, with the matrix that
        /// informs that <see cref="Context"/>.</remarks>
        public virtual DistanceMatrix Distances { get; set; }

        /// <summary>
        /// Renders a <see cref="string"/> Message informing a Validation
        /// <see cref="Exception"/>.
        /// </summary>
        /// <param name="pairs">The Pairs being combined into a message.</param>
        /// <returns></returns>
        /// <see cref="string.Join"/>
        protected static string RenderOutOfRangeMessage(params (string key, object value)[] pairs)
        {
            string RenderPair((string key, object value) pair) => $"{pair.key}: {pair.value}";
            return $"{{ {string.Join(", ", pairs.Select(RenderPair))} }} out of range";
        }

        /// <summary>
        /// Renders the <typeparamref name="T"/> <paramref name="values"/> Array
        /// with surrounding <paramref name="enclosure"/> characters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enclosure"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected static string RenderArray<T>(string enclosure, params T[] values)
        {
            string RenderValue(T value) => $"{value}";
            return string.Join(string.Join(", ", values.Select(RenderValue)), enclosure.ToArray());
        }

        /// <summary>
        /// Returns the <see cref="Range"/> of <see cref="IEnumerable{T}"/>
        /// corresponding to the <paramref name="values"/>.
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
        /// Renders the <typeparamref name="T"/> <paramref name="values"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        protected static string RenderArray<T>(params T[] values) => RenderArray("[]", values);

        /// <summary>
        /// Gets the Dimensions for Internal use.
        /// </summary>
        /// <remarks>It is a tiny bit verbose, we do admit, yet for type reasons
        /// we must shoe in Dimensions in this manner.</remarks>
        internal virtual ICollection<IDimension> InternalDimensions { get; } = new List<IDimension>();

        /// <summary>
        /// Gets the Dimensions.
        /// </summary>
        public virtual IEnumerable<IDimension> Dimensions => this.InternalDimensions;

        /// <summary>
        /// Gets whether the object IsDisposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Disposes of the object.
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
