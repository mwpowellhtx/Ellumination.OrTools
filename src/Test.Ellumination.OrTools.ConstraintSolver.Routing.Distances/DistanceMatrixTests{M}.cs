using System.Linq;
using System.Collections.Generic;

namespace Ellumination.OrTools.ConstraintSolver.Routing.Distances
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;
    using static Matrix;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="M"></typeparam>
    public abstract class DistanceMatrixTests<M> : MatrixTests<M>
        where M : DistanceMatrix, new()
    {
        protected DistanceMatrixTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Gets the Expected Length.
        /// </summary>
        protected virtual int ExpectedLength { get; }

        /// <inheritdoc/>
        protected override int ExpectedWidth => this.ExpectedLength;

        /// <inheritdoc/>
        protected override int ExpectedHeight => this.ExpectedLength;

        /// <inheritdoc/>
        protected override bool ExpectedMirrored { get; } = true;

        /// <summary>
        /// Verifies the <paramref name="matrix"/> value at the <paramref name="index"/>.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="index"></param>
        /// <param name="expected"></param>
        protected override void VerifyMatrixValue(M matrix, (int x, int y) index, int? expected = null)
        {
            expected = index.x == index.y ? default(int) : expected;
            matrix[index.x, index.y].AssertEqual(expected);
        }

        /// <inheritdoc/>
        protected override void OnVerifyMatrixBeforeBackground(out M instance, out IEnumerable<(int x, int y)> instanceIndices)
        {
            base.OnVerifyMatrixBeforeBackground(out instance, out instanceIndices);
        }

        /// <inheritdoc/>
        protected override void OnVerifyMatrixAfterBackground(out M instance, out IEnumerable<(int x, int y)> instanceIndices)
        {
            base.OnVerifyMatrixAfterBackground(out instance, out instanceIndices);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="instanceIndices"></param>
        [Background]
        public void DistanceMatrixBackground(M instance, IEnumerable<(int x, int y)> instanceIndices)
        {
            $"Verify {nameof(this.ExpectedWidth)} equal to {nameof(this.ExpectedLength)}".x(
                () => this.ExpectedWidth.AssertEqual(this.ExpectedLength)
            );

            $"Verify {nameof(this.ExpectedHeight)} equal to {nameof(this.ExpectedLength)}".x(
                () => this.ExpectedHeight.AssertEqual(this.ExpectedLength)
            );

            $"Verify the {nameof(instance)}".x(() => instance = this.Instance.AssertNotNull());

            $"Verify the {nameof(instanceIndices)}".x(
                () => instanceIndices = this.InstanceIndices.AssertCollectionNotEmpty().ToArray()
            );
        }
    }
}
