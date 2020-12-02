namespace Ellumination.OrTools.ConstraintSolver.Routing.Distances
{
    using Xunit.Abstractions;

    /// <summary>
    /// 
    /// </summary>
    public abstract class MatrixOnlyTests : MatrixTests<Matrix>
    {
        protected MatrixOnlyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Override in order to specify how to create the <see cref="Instance"/>.
        /// </summary>
        /// <param name="instance"></param>
        protected override void OnCreateInstance(out Matrix instance) =>
            instance = new Matrix(this.ExpectedWidth, this.ExpectedHeight);
    }
}
