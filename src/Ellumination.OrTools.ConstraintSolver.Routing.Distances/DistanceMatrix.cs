using System;
using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.Distances
{
    // TODO: TBD: ditto Matrix re: Ellumination.Collections...
    // TODO: TBD: we could potentially host this having broader use cases...
    /// <summary>
    /// Represents basic distance matrix concerns.
    /// </summary>
    public class DistanceMatrix : Matrix
    {
        // TODO: TBD: may also validate IsSquare...
        /// <summary>
        /// Gets the Length.
        /// </summary>
        public virtual int Length => this.Width;

        /// <summary>
        /// Gets or Sets whether <see cref="DistanceMatrix"/> <see cref="IsMirrored"/>.
        /// Default assumes <c>true</c>, however, this may be configured to best meet
        /// consumer requirements.
        /// </summary>
        public override bool IsMirrored { get; set; } = true;

        /// <summary>
        /// Zero the Diagonal to the <paramref name="value"/> value.
        /// </summary>
        /// <param name="value"></param>
        private void ZeroDiagonal(int value = default)
        {
            void OnZeroDiagonal(int index) => this[index, index] = value;
            Enumerable.Range(0, this.Length).ToList().ForEach(OnZeroDiagonal);
        }

        /// <summary>
        /// Constructs a default empty distance matrix.
        /// </summary>
        /// <see cref="Matrix.DefaultLength"/>
        public DistanceMatrix()
            : this(DefaultLength)
        {
        }

        /// <summary>
        /// Constructs a <c>2</c> by distance matrix.
        /// </summary>
        /// <param name="length">The size of the matrix dimension.</param>
        public DistanceMatrix(int length)
            : this(length, default)
        {
        }

        /// <summary>
        /// Constructs the distance matrix given uniform <paramref name="length"/> dimensions.
        /// </summary>
        /// <param name="length"></param>
        /// <param name="zero">Optionally specify the Zero along the diagonal.</param>
        public DistanceMatrix(int length, int zero)
            : base(length, length)
        {
            this.ZeroDiagonal(zero);
        }

        /// <summary>
        /// Constructs the distance matric given a default <paramref name="values"/>.
        /// </summary>
        /// <param name="values"></param>
        public DistanceMatrix(int?[,] values)
            : this(values, default)
        {
        }

        /// <summary>
        /// Constructs the distance matrix given a default <paramref name="values"/>.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="zero">Optionally specify the Zero along the diagonal.</param>
        public DistanceMatrix(int?[,] values, int zero)
            : base(values)
        {
            this.ZeroDiagonal(zero);
        }

        /// <summary>
        /// <see cref="ICloneable"/> compatible constructor.
        /// </summary>
        /// <param name="other"></param>
        public DistanceMatrix(DistanceMatrix other)
            : base(other)
        {
        }

        // TODO: TBD: may override IsReady(int x, int y) ...
    }
}
