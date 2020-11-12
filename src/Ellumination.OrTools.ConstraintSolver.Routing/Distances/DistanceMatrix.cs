using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Ellumination.OrTools.ConstraintSolver.Routing.Distances
{
    /// <summary>
    /// 
    /// </summary>
    public class DistanceMatrix
    {
        public int?[,] _matrix;

        public int?[,] Matrix
        {
            get => this._matrix;
            set => this._matrix = value;
        }

        /// <summary>
        /// Gets whether the Matrix IsSquare.
        /// </summary>
        /// <remarks>We think that the Matrix should be Square, but we can verify that.</remarks>
        public bool IsSquare => this._matrix.GetLength(0) == this._matrix.GetLength(1);

        /// <summary>
        /// Public Constructor.
        /// </summary>
        /// <param name="z"></param>
        public DistanceMatrix(int z)
            : this(z, z)
        {
        }

        /// <summary>
        /// Public Constructor.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public DistanceMatrix(int x, int y)
        {
            this.Matrix = new int?[x, y];
        }

        /// <summary>
        /// Indexer. On Set, we know that the Matrix may be mirrored along the
        /// diagonal for optimum performance.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public int? this[int i, int j]
        {
            get => this._matrix[i, j];
            set
            {
                this._matrix[i, j] = value;

                if (i != j && this.IsSquare)
                {
                    this._matrix[j, i] = value;
                }
            }
        }

        /// <summary>
        /// Returns whether the Matrix IsReady concerning the Distance between
        /// <paramref name="i"/> and <paramref name="j"/>.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public virtual bool IsReady(int i, int j)
        {
            bool TryIsReady(int? a, int? b) => a != null && a == b;
            return TryIsReady(this[i, j], this[j, i]);
        }

        /// <summary>
        /// Returns whether the entire Matrix IsReady, meaning values have been assigned.
        /// </summary>
        /// <returns></returns>
        /// <remarks>At this level we can only assume whether the Matrix is square, so
        /// therefore we have to assume that it may not be square, even though, in context,
        /// we think that it should be.</remarks>
        public virtual bool IsReady()
        {
            var this_Matrix = this.Matrix;

            var length = (i: this_Matrix.GetLength(0), j: this_Matrix.GetLength(1));

            for (var i = 0; i < length.i; i++)
            {
                for (var j = 0; j < length.j; j++)
                {
                    if (this_Matrix[i, j] == null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
