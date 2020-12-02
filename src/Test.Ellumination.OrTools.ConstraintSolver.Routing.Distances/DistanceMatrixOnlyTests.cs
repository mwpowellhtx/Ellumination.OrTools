namespace Ellumination.OrTools.ConstraintSolver.Routing.Distances
{
    using Xunit.Abstractions;

    /// <summary>
    /// 
    /// </summary>
    public abstract class DistanceMatrixOnlyTests : DistanceMatrixTests<DistanceMatrix>
    {
        protected DistanceMatrixOnlyTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Override in order to specify how to create the <see cref="Instance"/>.
        /// </summary>
        /// <param name="instance"></param>
        protected override void OnCreateInstance(out DistanceMatrix instance) =>
            instance = new DistanceMatrix(this.ExpectedLength);
    }
}
