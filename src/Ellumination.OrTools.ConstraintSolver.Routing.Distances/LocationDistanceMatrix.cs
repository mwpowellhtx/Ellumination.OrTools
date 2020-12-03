using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.Distances
{
    /// <summary>
    /// Represents a <see cref="DistanceMatrix"/> that is oriented towards geo-centric
    /// <see cref="Locations"/> usage. It is not the intention of this Matrix implementation to
    /// permit in-situ, so to speak, <see cref="Locations"/> adjustments, additions, subtractions,
    /// divisions, whatever. If a new set of Locations is required, then we create a new one. If
    /// necessary, at times we allow for a <see cref="Merge(LocationDistanceMatrix)"/> to take
    /// place, between two sets of Locations.
    /// </summary>
    public class LocationDistanceMatrix : DistanceMatrix, IEquatable<LocationDistanceMatrix>
    {
        private IReadOnlyList<string> _locations;

        /// <summary>
        /// We want the <see cref="Locations"/> Distinct in a case insensitive manner.
        /// </summary>
        private class LocationComparer : EqualityComparer<string>
        {
            /// <summary>
            /// Gets an instance of the <see cref="LocationComparer"/>.
            /// </summary>
            internal static LocationComparer Comparer { get; } = new LocationComparer();

            /// <summary>
            /// Returns the Normalized <paramref name="s"/>.
            /// </summary>
            /// <param name="s"></param>
            /// <returns></returns>
            private static string Normalize(string s) => (s ?? string.Empty).ToLower();

            /// <inheritdoc/>
            public override bool Equals(string x, string y) => Normalize(x) == Normalize(y);

            /// <inheritdoc/>
            public override int GetHashCode(string obj) => Normalize(obj).GetHashCode();

            /// <summary>
            /// Private default constructor.
            /// </summary>
            private LocationComparer()
            {
            }
        }

        /// <summary>
        /// Gets the Locations involved in the DistanceMatrix.
        /// </summary>
        public IEnumerable<string> Locations
        {
            get => this._locations;
            set => this._locations = value.OrEmpty().Distinct(LocationComparer.Comparer)
                .OrderBy(s => s).AsReadOnly();
        }

        /// <summary>
        /// Constructs a LocationDistanceMatrix with no <see cref="Locations"/>.
        /// </summary>
        public LocationDistanceMatrix()
            : this(Array.Empty<string>())
        {
        }

        /// <summary>
        /// Constructs a LocationDistanceMatrix given a set of <paramref name="locations"/>.
        /// </summary>
        /// <param name="locations"></param>
        public LocationDistanceMatrix(IEnumerable<string> locations)
            : this(locations.OrEmpty().ToArray())
        {
        }

        /// <summary>
        /// Constructs a LocationDistanceMatrix given a set of <paramref name="locations"/>.
        /// </summary>
        /// <param name="locations"></param>
        public LocationDistanceMatrix(params string[] locations)
            : base(locations.OrEmpty().Distinct(LocationComparer.Comparer).Count())
        {
            this.Locations = locations;
        }

        /// <summary>
        /// <see cref="ICloneable"/> compabitle constructor.
        /// </summary>
        /// <param name="other"></param>
        public LocationDistanceMatrix(LocationDistanceMatrix other)
            : base(other)
        {
            // Ensures that we have a clone-friendly copy of the Other instance.
            this.Locations = other._locations;
        }

        /// <summary>
        /// Location based Indexer especially as a function of
        /// <see cref="CollectionExtensionMethods.IndexOf{T}(IReadOnlyList{T}, T)"/>
        /// and the base class <see cref="Matrix.this[int, int]"/>.
        /// </summary>
        /// <param name="x">The <see cref="Locations"/> wise X coordinate.</param>
        /// <param name="y">The <see cref="Locations"/> wise Y coordinate.</param>
        /// <returns></returns>
        public int? this[string x, string y]
        {
            get => this[this._locations.IndexOf(x), this._locations.IndexOf(y)];
            set => this[this._locations.IndexOf(x), this._locations.IndexOf(y)] = value;
        }

        /// <summary>
        /// Returns whether the DistanceMatrix IsReady concerning the Distance between
        /// <paramref name="x"/> and <paramref name="y"/> <see cref="Locations"/>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual bool IsReady(string x, string y) => !(this[x, y] == null || this[y, x] == null);

        /// <summary>
        /// Use the Key <see cref="EqualityComparer{T}"/> for purposes of reducing
        /// the number of Keys that we use in order to verify <see cref="IsReady"/>.
        /// </summary>
        private class KeyEqualityComparer : EqualityComparer<(string x, string y)>
        {
            /// <summary>
            /// Private default constructor.
            /// </summary>
            private KeyEqualityComparer()
            {
            }

            /// <summary>
            /// Gets the Comparer instance for Internal use.
            /// </summary>
            internal static KeyEqualityComparer Comparer { get; } = new KeyEqualityComparer();

            /// <inheritdoc/>
            public override bool Equals((string x, string y) a, (string x, string y) b) =>
                (a.x == b.x && a.y == b.y) || (a.x == b.y && a.y == b.x);

            /// <inheritdoc/>
            public override int GetHashCode((string x, string y) obj) => obj.GetHashCode();
        }

        /// <summary>
        /// Gets the DistinctKeys necessary to index both mirrored sides of the matrix
        /// along its identity diagonal.
        /// </summary>
        private IEnumerable<(string x, string y)> DistinctKeys => this.Locations
            .Zip(this.Locations, (x, y) => (x, y)).Distinct(KeyEqualityComparer.Comparer);

        /// <summary>
        /// Returns whether the Matrix as a whole IsReady.
        /// </summary>
        /// <returns></returns>
        /// <remarks>At this level we may also decide whether <see cref="Matrix.IsSquare"/>
        /// and whether the <see cref="Locations"/> are all aligned properly.</remarks>
        public override bool IsReady()
        {
            var keys = this.DistinctKeys.ToArray();

            // TODO: TBD: or do we simply assume that the identity diagonal is always zero?
            return this.IsSquare && base.IsReady() && keys.All(key =>
                (key.x == key.y && this[key.x, key.y] == default)
                    || this[key.x, key.y] == this[key.y, key.x]);
        }

        /// <inheritdoc/>
        public override bool Equals(Matrix other) => (
            other is LocationDistanceMatrix location
                && Equals(location) ) || base.Equals(other)
            ;

        /// <summary>
        /// Returns whether <paramref name="a"/> Equals <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <see cref="Matrix.Equals(Matrix, Matrix)"/>
        public static bool Equals(LocationDistanceMatrix a, LocationDistanceMatrix b) =>
            Matrix.Equals(a, b) && a.Locations.SequenceEqual(b.Locations)
            ;

        /// <inheritdoc/>
        public virtual bool Equals(LocationDistanceMatrix other) => Equals(this, other);

        /// <summary>
        /// Return the <see cref="LocationDistanceMatrix"/> based on the contributions
        /// Merging <paramref name="a"/> with <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static LocationDistanceMatrix Merge(LocationDistanceMatrix a, LocationDistanceMatrix b)
        {
            // Which both inform the DistinctKeys of the contributing matrices, but we do not need the keys.
            var result = new LocationDistanceMatrix(a.Locations.Concat(b.Locations).ToArray());

            int? GetMergeValue(LocationDistanceMatrix matrix, (string x, string y) key) =>
                !new[] { key.x, key.y }.All(matrix.Locations.Contains) ? null : matrix[key.x, key.y];

            void OnMergeDistinctKey((string x, string y) key) =>
                result[key.x, key.y] = GetMergeValue(a, key) ?? GetMergeValue(b, key);

            result.DistinctKeys.ToList().ForEach(OnMergeDistinctKey);

            return result;
        }

        /// <summary>
        /// Merges this Matrix with the <paramref name="other"/> Matrix.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual LocationDistanceMatrix Merge(LocationDistanceMatrix other) => Merge(this, other);
    }
}
