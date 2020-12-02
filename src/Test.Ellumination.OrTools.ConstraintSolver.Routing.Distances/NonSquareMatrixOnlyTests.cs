namespace Ellumination.OrTools.ConstraintSolver.Routing.Distances
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;

    /// <summary>
    /// 
    /// </summary>
    public class NonSquareMatrixOnlyTests : MatrixOnlyTests
    {
        public NonSquareMatrixOnlyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <inheritdoc/>
        protected override int ExpectedWidth { get; } = 3;

        /// <inheritdoc/>
        protected override int ExpectedHeight { get; } = 2;

        /// <inheritdoc/>
        protected override void OnCreateInstance(out Matrix instance) =>
            instance = new Matrix(this.ExpectedWidth, this.ExpectedHeight);

        /// <summary>
        /// 
        /// </summary>
        [Background]
        public void NonSquareMatrixOnlyBackground()
        {
            $"Verify this.{this.ExpectedSquare}".x(() => this.ExpectedSquare.AssertFalse());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        [Scenario]
        public void Verify(Matrix instance, int width, int height)
        {
            $"Obtain the {nameof(instance)}".x(() => instance = this.Instance);

            $"Obtain the {nameof(width)}".x(() => width = this.ExpectedWidth);

            $"Obtain the {nameof(height)}".x(() => height = this.ExpectedHeight);

            // Which should all be verified already, but we will do so for sake of closing the loop.
            $"Verify {nameof(instance)}.{nameof(instance.Width)}".x(() => instance.AssertNotNull().Width.AssertEqual(width));

            $"Verify {nameof(instance)}.{nameof(instance.Height)}".x(() => instance.AssertNotNull().Height.AssertEqual(height));
        }
    }
}
