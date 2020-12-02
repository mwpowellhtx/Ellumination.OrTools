using System.Linq;

namespace Ellumination.OrTools.ConstraintSolver.Routing.Distances
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved;

    /// <summary>
    /// 
    /// </summary>
    public class SquareDistanceMatrixTests : DistanceMatrixOnlyTests
    {
        public SquareDistanceMatrixTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <inheritdoc/>
        protected override int ExpectedLength { get; } = 7;

        [Background]
        public void SquareBackground()
        {
            $"Verify square".x(() => this.Instance.IsSquare.AssertTrue());
        }

        [Scenario]
        public void Diagonal_Should_Be_Zero()
        {
            const int zero = default;

            void OnVerifyDiagonal()
            {
                var instance = this.Instance.AssertNotNull();
                void OnVerifyEach(int i) => instance[i, i].AssertNotNull().AssertEqual(zero);
                Enumerable.Range(0, this.ExpectedLength).ToList().ForEach(OnVerifyEach);
            }

            $"Verify diagonal bits all zero".x(OnVerifyDiagonal);
        }

        [Scenario]
        public void Non_Diagonal_Each_Null()
        {
            void OnVerifyNonDiagonal()
            {
                var instance = this.Instance.AssertNotNull();
                void OnVerifyEach((int x, int y) index) => instance[index.x, index.y].AssertNull();
                this.InstanceIndices.Where(index => index.x != index.y).ToList().ForEach(OnVerifyEach);
            }

            $"Verify non diagonal bits all null".x(OnVerifyNonDiagonal);
        }
    }
}
