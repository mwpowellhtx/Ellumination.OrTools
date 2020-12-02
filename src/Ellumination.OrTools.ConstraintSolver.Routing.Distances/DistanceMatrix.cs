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
        /// <see cref="DefaultLength"/>
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
        /// Constructs the distance matrix given a default <paramref name="values"/>.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="zero">Optionally specify the Zero along the diagonal.</param>
        public DistanceMatrix(int?[,] values, int zero = default)
            : base(values)
        {
            this.ZeroDiagonal(zero);
        }

        // TODO: TBD: may override IsReady(int x, int y) ...

        /// <inheritdoc/>
        public override int? this[int x, int y]
        {
            get => base[x, y];
            set
            {
                base[x, y] = value;

                // TODO: TBD: simply validate whether this.IsSquare...
                if (x != y && !this.IsSquare)
                {
                    base[y, x] = value;
                }
            }
        }
    }
}
