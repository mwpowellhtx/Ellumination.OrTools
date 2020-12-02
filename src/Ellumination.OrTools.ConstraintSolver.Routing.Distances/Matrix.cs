using System;
using System.Collections.Generic;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.Distances
{
    // TODO: TBD: this has borderline much broader use cases...
    // TODO: TBD: potentially we can host this in Ellumination.Collections instead...
    /// <summary>
    /// Represents basic Matrix concerns.
    /// </summary>
    public class Matrix
    {
        /// <summary>
        /// Gets the Values associated with the Matrix.
        /// </summary>
        public virtual int?[,] Values { get; set; }

        /// <summary>
        /// Gets the Values Width.
        /// </summary>
        /// <see cref="Values"/>
        public virtual int Width => this.Values?.GetLength(0) ?? default;

        /// <summary>
        /// Gets the Values Height.
        /// </summary>
        /// <see cref="Values"/>
        public virtual int Height => this.Values?.GetLength(1) ?? default;

        /// <summary>
        /// Gets whether the Matrix IsSquare.
        /// </summary>
        /// <remarks>We think that the Matrix should be Square, but we can verify that.</remarks>
        /// <see cref="Width"/>
        /// <see cref="Height"/>
        public virtual bool IsSquare => this.Width == this.Height;

        /// <summary>
        /// Gets or Sets whether the Matrix IsMirrored. This is for use when
        /// <see cref="Matrix"/> is also <see cref="IsSquare"/>. When a
        /// <see cref="Values"/> is set via the indexer and <see cref="IsMirrored"/>
        /// is set to <c>true</c>, then the mirror value is also set.
        /// </summary>
        public virtual bool IsMirrored { get; set; } = default;

        /// <summary>
        /// <c>2</c>
        /// </summary>
        internal const int DefaultLength = 2;

        /// <summary>
        /// Constructs a <c>2</c> by matrix.
        /// </summary>
        /// <see cref="DefaultLength"/>
        public Matrix()
            : this(DefaultLength)
        {
        }

        /// <summary>
        /// Constructs the matrix given uniform <paramref name="length"/> dimensions.
        /// </summary>
        /// <param name="length"></param>
        public Matrix(int length)
            : this(length, length)
        {
        }

        /// <summary>
        /// Initializes the <paramref name="values"/>.
        /// </summary>
        /// <param name="values"></param>
        /// <see cref="Values"/>
        private void Initialize(int?[,] values) => this.Values = values;

        // TODO: TBD: width/height bits might make sense if we had a general use Matrix...
        /// <summary>
        /// Constructs the matrix given <paramref name="width"/> and <paramref name="height"/>
        /// dimensions.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Matrix(int width, int height) => this.Initialize(new int?[width, height]);

        /// <summary>
        /// Constructs the matrix given a default <paramref name="values"/>.
        /// </summary>
        /// <param name="values"></param>
        public Matrix(int?[,] values) => this.Initialize(values);

        /// <summary>
        /// Indexer gets or sets the value in the Matrix.
        /// </summary>
        /// <param name="x">The <see cref="Width"/> wise X coordinate.</param>
        /// <param name="y">The <see cref="Height"/> wise Y coordinte.</param>
        /// <returns></returns>
        public virtual int? this[int x, int y]
        {
            get => this.Values[x, y];
            set
            {
                this.Values[x, y] = value;

                if (x != y && this.IsMirrored && this.IsSquare)
                {
                    this.Values[y, x] = value;
                }
            }
        }

        /// <summary>
        /// Returns whether the Matrix IsReady concerning the <see cref="Width"/> and
        /// <see cref="Height"/> wise <paramref name="x"/> and <paramref name="y"/>
        /// coordinates, respectively.
        /// </summary>
        /// <param name="x">The <see cref="Width"/> wise X coordinate.</param>
        /// <param name="y">The <see cref="Height"/> wise Y coordinte.</param>
        /// <returns></returns>
        public virtual bool IsReady(int x, int y) => this[x, y] != null;

        /// <summary>
        /// Returns whether the entire Matrix IsReady, meaning values have been assigned.
        /// </summary>
        /// <returns></returns>
        /// <remarks>At this level we can only assume whether the Matrix is square, so
        /// therefore we have to assume that it may not be square, even though, in context,
        /// we think that it should be.</remarks>
        public virtual bool IsReady()
        {
            IEnumerable<int> Range(int count) => Enumerable.Range(0, count);

            // Not Any that are Not Ready, more efficient than evaluating All are Ready.
            return !Range(this.Width).SelectMany(x => Range(this.Height).Select(y => (x, y)))
                .Any(z => !IsReady(z.x, z.y));
        }
    }
}
