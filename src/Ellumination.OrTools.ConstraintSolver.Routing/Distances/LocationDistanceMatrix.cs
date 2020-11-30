using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Ellumination.OrTools.ConstraintSolver.Routing.Distances
{
    public class LocationDistanceMatrix : DistanceMatrix
    {
        private HashSet<string> _locations;

        /// <summary>
        /// G
        /// </summary>
        public HashSet<string> Locations
        {
            get => this._locations;
            set => this._locations = value.OrEmpty().ToHashSet();
        }

        public LocationDistanceMatrix(params string[] locations)
            : base(locations.Distinct().Count())
        {
            this.Locations = locations.Distinct().ToHashSet();

            /* We know that no matter what else, the Identity diagonal is always zero.
             * In other words, a location distance to itself is ZERO. ALWAYS. */

            Enumerable.Range(0, this.Locations.Count).ToList().ForEach(i => this[i, i] = default);
        }

        /// <summary>
        /// Returns the Index associated with the <paramref name="key"/> given the
        /// <see cref="Locations"/>. Returns <c>-1</c> when there are no matches
        /// aligned with the <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int this[string key]
        {
            get
            {
                // Which the hash set should offer some uniqueness to begin with, then we also align the key.
                int CalculateIndex(params (bool found, int i)[] matches) => matches.Any() ? matches.Single().i : -1;
                return CalculateIndex(this.Locations.Select((x, i) => (found: x == key, i)).Where(z => z.found).ToArray());
            }
        }

        /// <summary>
        /// Returns the Value associated with the <paramref name="index"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <see cref="DistanceMatrix.this[int, int]"/>
        private int? Get((int i, int j) index) => this[index.i, index.j];

        /// <summary>
        /// Sets the <paramref name="value"/> associated with the <paramref name="index"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <see cref="DistanceMatrix.this[int, int]"/>
        private void Set((int i, int j) index, int? value) => this[index.i, index.j] = value;

        /// <summary>
        /// Location based Indexer especially as a function of <see cref="this[string]"/>
        /// and the base class <see cref="DistanceMatrix.this[int, int]"/>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        /// <see cref="this[string]"/>
        /// <see cref="Get"/>
        /// <see cref="Set"/>
        public int? this[string x, string y]
        {
            get => this.Get((this[x], this[y]));
            set => this.Set((this[x], this[y]), value);
        }

        /// <summary>
        /// Returns whether the Matrix IsReady concerning the Distance between
        /// <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual bool IsReady(string x, string y)
        {
            bool TryIsReady(int? a, int? b) => a != null && a == b;
            return TryIsReady(this[x, y], this[y, x]);
        }

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
        /// <remarks>At this level we may also decide whether
        /// <see cref="DistanceMatrix.IsSquare"/> and whether the
        /// <see cref="Locations"/> are all aligned properly.</remarks>
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
