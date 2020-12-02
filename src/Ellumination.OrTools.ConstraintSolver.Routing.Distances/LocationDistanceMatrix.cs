using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.Distances
{
    // TODO: TBD: this one has borderline broader use cases...
    // TODO: TBD: whether that is general enough for Ellumination.Collections...
    // TODO: TBD: since after all it is more geared to geo-centric distance matrices...
    /// <summary>
    /// Represents a <see cref="DistanceMatrix"/> that is oriented towards
    /// geo-centric <see cref="Locations"/> usage.
    /// </summary>
    public class LocationDistanceMatrix : DistanceMatrix
    {
        private HashSet<string> _locations;

        /// <summary>
        /// Gets the Locations involved in the DistanceMatrix.
        /// </summary>
        public HashSet<string> Locations
        {
            get => this._locations;
            set
            {
                // TODO: TBD: which should also have a potentially precipitous effect on the underlying Matrix Values...
                // TODO: TBD: either preservative, additional or deletorious effects...
                // TODO: TBD: especially for subset, equal (which is a kind of subset), or superset use cases...
                this._locations = value.OrEmpty().ToHashSet();
            }
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
            : base(locations.Distinct().Count())
        {
            this.Locations = locations.Distinct().ToHashSet();
        }

        /// <summary>
        /// <see cref="ICloneable"/> compabitle constructor.
        /// </summary>
        /// <param name="other"></param>
        public LocationDistanceMatrix(LocationDistanceMatrix other)
            : base(other)
        {
            // Ensures that we have a clone-friendly copy of the Other instance.
            this._locations = other._locations.OrEmpty().ToHashSet();
        }

        /// <summary>
        /// Returns the IndexOf the <paramref name="key"/> with respect to the
        /// <see cref="Locations"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual int IndexOf(string key)
        {
            // Which the hash set should offer some uniqueness to begin with, then we also align the key.
            int OnGetIndexOf(params (bool found, int i)[] matches) => matches.Any() ? matches.Single().i : -1;
            return OnGetIndexOf(this.Locations.Select((x, i) => (found: x == key, i)).Where(z => z.found).ToArray());
        }

        /// <summary>
        /// Location based Indexer especially as a function of <see cref="IndexOf"/>
        /// and the base class <see cref="Matrix.this[int, int]"/>.
        /// </summary>
        /// <param name="x">The <see cref="Locations"/> wise X coordinate.</param>
        /// <param name="y">The <see cref="Locations"/> wise Y coordinate.</param>
        /// <returns></returns>
        public int? this[string x, string y]
        {
            get => this[this.IndexOf(x), this.IndexOf(y)];
            set => this[this.IndexOf(x), this.IndexOf(y)] = value;
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
            private KeyEqualityComparer()
            {
            }

            internal static KeyEqualityComparer Comparer { get; } = new KeyEqualityComparer();

            public override bool Equals((string x, string y) a, (string x, string y) b) =>
                (a.x == b.x && a.y == b.y) || (a.x == b.y && a.y == b.x);

            public override int GetHashCode((string x, string y) obj) =>
                obj.x.GetHashCode() ^ (obj.y.GetHashCode() * 7)
                    + obj.y.GetHashCode() ^ (obj.x.GetHashCode() * 7);
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
